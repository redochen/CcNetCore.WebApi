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
                .OrderBy (p => GetColumnAttribute (p)?.Order ?? 0);

            for (int i = 0, count = initProperties.Count (); i < count; i++) {
                var property = initProperties.ElementAt (i);
                var isKey = keyProperties.Contains (property);
                var isExplicitKey = explicitKeyProperties.Contains (property);
                var attribute = GetColumnAttribute (property);
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
        /// <param name="connection"></param>
        /// <param name="getVerbSql"></param>
        /// <param name="getWhereSql"></param>
        /// <param name="getSetSql"></param>
        /// <param name="getExtWhereSql"></param>
        /// <returns></returns>
        private static (string, DynamicParameters) GetSql (this IDbConnection connection, Func<string> getVerbSql,
            Func<ISqlAdapter /*adapter*/ , StringBuilder /*sbWhere*/ , DynamicParameters /*dyncParms*/ , string> getWhereSql,
            Func<ISqlAdapter /*adapter*/ , DynamicParameters /*dyncParms*/ , string> getSetSql = null,
            Action<ISqlAdapter /*adapter*/ , StringBuilder /*sbSql*/> getExtWhereSql = null) {
            var adapter = GetFormatter (connection);
            var dyncParms = new DynamicParameters ();

            //must invoked before getWhereSql
            var setSql = getSetSql?.Invoke (adapter, dyncParms).GetValue ();

            var sbWhere = new StringBuilder ();
            var tableName = getWhereSql (adapter, sbWhere, dyncParms);

            var sbSql = new StringBuilder ();

            //select * from table
            //delete from table
            //update table set xxx=yyy
            sbSql.Append ($"{getVerbSql()} {tableName} {setSql}");

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

                sbSet.Append (adapter.GetColumnNameEqualsValue (property), Chars.逗号);
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
        /// <param name="matchSql">WHERE语句</param>
        /// <param name="parameters">参数列表</param>
        /// <typeparam name="T"></typeparam>
        private static string GetWhereSql<T> (ISqlAdapter adapter, StringBuilder sbWhere, DynamicParameters dyncParms,
            string matchSql, Dictionary<string, object> parameters) where T : class, new () {
            sbWhere.Append (matchSql);

            var tableName = HandleMatchFields<T> (autoMatch: false,
                getValue: (property, key) => {
                    if (!key.IsValid () || !parameters.ContainsKey (key)) {
                        return null;
                    }

                    return parameters[key];
                },
                handleMatch: (index, property, key, value) => {
                    var i = index * 2;
                    sbWhere.Replace ($"{{{i}}}", property.GetColumnName ());
                    sbWhere.Replace ($"{{{i + 1}}}", $"@{key}");
                    dyncParms.Add ($"@{key}", value);
                }, parameters.Keys.ToArray ());

            return tableName;
        }

        /// <summary>
        /// 获取WHERE语句
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="sbWhere"></param>
        /// <param name="dyncParms"></param>
        /// <param name="opCode"></param>
        /// <param name="entity"></param>
        /// <param name="matchSql"></param>
        /// <param name="Match"></param>
        /// <param name="matchFields"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetWhereSql<T> (ISqlAdapter adapter, StringBuilder sbWhere, DynamicParameters dyncParms,
            string opCode, T entity, string matchSql, bool autoMatch, params string[] matchFields)
        where T : class, new () {
            if (!opCode.IsValid ()) {
                sbWhere.Append (matchSql);
            }

            var tableName = HandleMatchFields<T> (autoMatch: autoMatch,
                getValue: (poperty, key) => poperty.GetValue (entity),
                handleMatch: (index, poperty, key, value) => {
                    var matchExp = adapter.GetColumnNameEqualsValue (poperty);

                    if (opCode.IsValid ()) {
                        if (sbWhere.Length > 0) {
                            sbWhere.Append ($" {opCode} ");
                        }
                        sbWhere.Append (matchExp);
                    } else {
                        sbWhere.Replace ($"{{{index}}}", matchExp);
                    }

                    dyncParms.Add ($"@{key}", value);
                }, matchFields);

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
            params string[] matchFields) where T : class, new () {
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

            properties.ForEach (p => {
                var addWhere = false;
                var value = getValue (p, p.Name);

                if (p.PropertyType.IsNullableType ()) {
                    addWhere = (value != null);
                } else {
                    addWhere = value.IsValid (CheckValidFlag.Default);
                }

                if (addWhere) {
                    handleMatch (index++, p, p.Name, value);
                }
            });

            return tableName;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="matchSql"></param>
        /// <returns></returns>
        private static string GetOpCode (string matchSql) {
            var opCode = new string[] { MatchSql.AND, MatchSql.OR }.FirstOrDefault (
                x => x.EqualsEx (matchSql, ignoreCase : true));
            return opCode;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="properity"></param>
        /// <returns></returns>
        private static ColumnAttribute GetColumnAttribute (PropertyInfo properity) {
            if (null == properity) {
                return null;
            }

            var columnAttr = ColumnNameProperitiesCache (properity.ReflectedType) ?
                .Where (p => p.Name.Equals (properity.Name)) ?
                .Select (p => p.GetAttribute<ColumnAttribute> (true)).FirstOrDefault ();
            return columnAttr;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetColumnName (this PropertyInfo property) {
            var attr = GetColumnAttribute (property);
            return attr?.Name ?? property?.Name;
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