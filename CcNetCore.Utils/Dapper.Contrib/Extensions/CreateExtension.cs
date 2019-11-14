#pragma warning disable CS0168

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// The Dapper.Contrib extensions for Dapper
    /// </summary>
    public static partial class SqlMapperExtensions {
        /// <summary>
        /// 数据表不存时创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool CreateIfNotExists<T> (this IDbConnection connection) where T : class {
            var type = typeof (T);

            var keyProperties = KeyPropertiesCache (type).ToList (); //added ToList() due to issue #418, must work on a list copy
            var explicitKeyProperties = ExplicitKeyPropertiesCache (type);
            if (keyProperties.Count == 0 && explicitKeyProperties.Count == 0) {
                throw new ArgumentException ("Entity must have at least one [Key] or [ExplicitKey] property");
            }

            var name = GetTableName (type);

            var adapter = GetFormatter (connection);
            if (adapter.ExistsTable (connection, name)) {
                return true;
            }

            var sbSql = new StringBuilder ();
            sbSql.AppendFormat ("create table {0} (", name);

            var allProperties = TypePropertiesCache (type);
            var requiredProperties = RequiredPorpertiesCache (type);
            var ignoredProperties = IgnoredPropertiesCache (type);
            var autoIncrementProperties = AutoIncrementPropertiesCache (type);
            var initProperties = allProperties.Except (ignoredProperties)
                .OrderBy (p => PropertyColumnsCache (p)?.Order ?? 0);

            for (int i = 0, count = initProperties.Count (); i < count; i++) {
                var property = initProperties.ElementAt (i);
                var isKey = keyProperties.Contains (property);
                var isExplicitKey = explicitKeyProperties.Contains (property);
                var attribute = PropertyColumnsCache (property);
                var columnName = attribute?.Name ?? property.Name;
                var columnType = property.PropertyType.GetUnderlyingType ();
                var isNotNull = (!property.PropertyType.IsNullableType ()) && (requiredProperties.Contains (property));
                var autoIncrement = autoIncrementProperties?.Contains (property) ?? false;

                adapter.AppendColumnDefination (sbSql, columnName, columnType, attribute,
                    isExplicitKey, isKey, isNotNull, autoIncrement);
                if (i < count - 1) {
                    sbSql.Append (",");
                }
            }

            sbSql.Append (")");

            var state = connection.Execute (sbSql.ToString ());
            return state > 0;
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="predicate"></param>
        /// <param name="getSetSql"></param>
        /// <param name="getExtWhereSql"></param>
        /// <returns></returns>
        private static (string, DynamicParameters) GetSql<T> (this Command cmd, SqlPredicate<T> predicate,
            Func<ISqlAdapter /*adapter*/ , DynamicParameters /*dyncParms*/ , string> getSetSql = null,
            Action<ISqlAdapter /*adapter*/ , StringBuilder /*sbSql*/> getExtWhereSql = null)
        where T : class, new () {
            var adapter = GetFormatter (cmd.Connection);
            var dyncParms = new DynamicParameters ();

            //must invoked before getWhereSql
            var setSql = getSetSql?.Invoke (adapter, dyncParms).GetValue ();

            var sbWhere = new StringBuilder ();
            var tableName = GetWhereSql (adapter, sbWhere, dyncParms, predicate);
            var sbSql = new StringBuilder ();

            //select * from table
            //delete from table
            //update table set xxx=yyy
            sbSql.Append ($"{cmd.Verb} {tableName} {setSql}");

            if (sbWhere.Length > 0) {
                sbSql.Append ($" where {sbWhere.ToString()}");
            }

            getExtWhereSql?.Invoke (adapter, sbSql);

            return (sbSql.ToString (), dyncParms);
        }

        /// <summary>
        /// 获取SET语句
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="dyncParms"></param>
        /// <param name="value"></param>
        /// <param name="updateFields"></param>
        /// <typeparam name="T"></typeparam>
        private static string GetSetSql<T> (ISqlAdapter adapter, DynamicParameters dyncParms,
            T value, IEnumerable<string> updateFields) where T : class, new () {
            var type = typeof (T);
            var properties = TypePropertiesCache (type);
            var sbSet = new StringBuilder ();

            for (int i = 0, count = updateFields.Count (); i < count; ++i) {
                var field = updateFields.ElementAt (i);
                var property = properties.FirstOrDefault (p => p.Name.Equals (field));
                if (null == property) {
                    continue;
                }

                sbSet.Append (adapter.GetColumnMatchesValue (property.GetColumnName (),
                    property.Name, MatchType.Equal), Chars.逗号);
                dyncParms.Add ($"@{property.Name}", property.GetValue (value));
            }

            return $"set {sbSet.ToString()}";
        }

        /// <summary>
        /// 获取WHERE语句
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="sbWhere"></param>
        /// <param name="dyncParms"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static string GetWhereSql<T> (ISqlAdapter adapter, StringBuilder sbWhere,
            DynamicParameters dyncParms, SqlPredicate<T> predicate) where T : class, new () {
            if (predicate?.SqlTemplate.IsValid () ?? false) {
                sbWhere.Append (predicate.SqlTemplate);
            }

            var tableName = HandleMatchFields<T> (autoMatch: predicate?.AutoMatch ?? true,
                getValue: (property, key) => {
                    if (!(predicate?.Parameters.IsEmpty () ?? true)) {
                        if (!key.IsValid () || !predicate.Parameters.ContainsKey (key)) {
                            return null;
                        }
                        return predicate.Parameters[key];
                    } else {
                        return property.GetValue (predicate?.Condition);
                    }
                },
                handleMatch: (index, property, key, value) => {
                    var matchType = predicate?.GetMatchType?.Invoke (property) ?? MatchType.Equal;
                    var matchExp = adapter.GetColumnMatchesValue (
                        property.GetColumnName (), property.Name, matchType);

                    if (predicate?.LogicType.IsValid () ?? false) {
                        if (sbWhere.Length > 0) {
                            sbWhere.Append ($" {predicate.LogicType} ");
                        }
                        sbWhere.Append (matchExp);
                    } else {
                        sbWhere.Replace ($"{{{index}}}", matchExp);
                    }

                    dyncParms.Add ($"@{key}", HandleLikeValue (matchType, value));
                }, predicate?.MatchFields);

            return tableName;
        }

        /// <summary>
        /// 处理匹配字段
        /// </summary>
        /// <param name="autoMatch">如果matchFields为空，是否自动识别匹配字段</param>
        /// <param name="getValue">获取字段值的方法</param>
        /// <param name="handleMatch">处理匹配字段的方法</param>
        /// <param name="matchFields">匹配字段列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>数据表名称</returns>
        private static string HandleMatchFields<T> (bool autoMatch,
            Func<PropertyInfo /*property*/ , string /*key*/ , object /*value*/> getValue,
            Action<int /*index*/ , PropertyInfo /*property*/ , string /*key*/ , object /*value*/> handleMatch,
            IEnumerable<string> matchFields) where T : class, new () {
            var type = typeof (T);

            var index = 0;
            //GetSingleKey<T> (nameof (GetWhere));
            var tableName = GetTableName (type);
            var properties = TypePropertiesCache (type);

            if (!matchFields.IsEmpty ()) {
                for (int i = 0, count = matchFields.Count (); i < count; ++i) {
                    var field = matchFields.ElementAt (i);
                    var property = properties.FirstOrDefault (p => p.Name.Equals (field));
                    if (null == property) {
                        continue;
                    }

                    var value = getValue (property, property.Name);
                    handleMatch (index++, property, property.Name, value);
                }

                return tableName;
            }

            if (!autoMatch) {
                return tableName;
            }

            properties.ForEach (property => {
                var addWhere = false;
                var value = getValue (property, property.Name);

                if (property.PropertyType.IsNullableType ()) {
                    addWhere = (value != null);
                } else {
                    addWhere = value.IsValid (CheckValidFlag.Default);
                }

                if (addWhere) {
                    handleMatch (index++, property, property.Name, value);
                }
            });

            return tableName;
        }

        /// <summary>
        /// 处理Like匹配的值
        /// </summary>
        /// <param name="matchType"></param>
        /// <param name="value"></param>
        /// <param name="likeValue"></param>
        /// <returns></returns>
        private static object HandleLikeValue (MatchType matchType, object value) {
            var isLike = false;
            var sbValue = new StringBuilder ();

            if (matchType == MatchType.Like || matchType == MatchType.NotLike ||
                matchType == MatchType.EndsWith || matchType == MatchType.NotEndsWith) {
                isLike = true;
                sbValue.Append (Chars.百分符);
            }

            sbValue.Append (value);

            if (matchType == MatchType.Like || matchType == MatchType.NotLike ||
                matchType == MatchType.BeginsWith || matchType == MatchType.NotBeginsWith) {
                isLike = true;
                sbValue.Append (Chars.百分符);
            }

            return isLike ? sbValue.ToString () : value;
        }

        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetColumnName (this PropertyInfo property) {
            var attr = PropertyColumnsCache (property);
            return attr?.Name.GetValue (property?.Name);
        }

        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="field"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetColumnName<T> (string field) {
            var property = TypePropertiesCache (typeof (T))?.FirstOrDefault (
                p => p.Name.Equals (field));
            return property.GetColumnName ();
        }

        /// <summary>
        /// 将数据映射成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        private static T DataToEntity<T> (IDictionary<string, object> data, bool ignoreCase = true)
        where T : class, new () {
            T obj = default (T);

            try {
                var type = typeof (T);
                obj = type.IsInterface ? ProxyGenerator.GetInterfaceProxy<T> () : new T ();

                var allProperties = TypePropertiesCache (type);
                var converterProperties = ConverterPropertiesCache (type);

                foreach (var property in allProperties) {
                    var columnName = property.GetColumnName ();
                    var kvp = data.FirstOrDefault (x => x.Key.EqualsEx (columnName, ignoreCase : ignoreCase));
                    var val = kvp.Value;
                    if (null == kvp.Value) {
                        continue;
                    }

                    TypeConverter converter = null;
                    var converterAttr = converterProperties.FirstOrDefault (ca => ca.Property == property);
                    if (converterAttr.Attribute != null) {
                        converter = GetTypeConverter (converterAttr.Attribute);
                    }

                    var memberType = property.GetMemberType (retriveUnderlyingType: true);
                    if (memberType != null) {
                        val = val.ChangeType (memberType, out string error, converter);
                        property.SetValue (obj, val, null);
                    } else {
                        obj.SetMemberValue (property, val);
                    }
                }

                if (obj is IProxy p) {
                    //reset change tracking and return
                    p.IsDirty = false;
                }
            } catch (Exception ex) { }

            return obj;
        }
    }
}