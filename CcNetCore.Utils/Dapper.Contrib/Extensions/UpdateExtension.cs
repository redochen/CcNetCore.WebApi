using System;
using System.Data;
using System.Linq;
using System.Text;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// The Dapper.Contrib extensions for Dapper
    /// </summary>
    public static partial class SqlMapperExtensions {
        /// <summary>
        /// Updates entity in table "Ts", checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToUpdate">Entity to be updated</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public static bool Update<T> (this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class {
            if (entityToUpdate is IProxy proxy && !proxy.IsDirty) {
                return false;
            }

            var type = typeof (T);

            if (type.IsArray) {
                type = type.GetElementType ();
            } else if (type.IsGenericType) {
                //var typeInfo = type.GetTypeInfo();
                //bool implementsGenericIEnumerableOrIsGenericIEnumerable =
                //    typeInfo.ImplementedInterfaces.Any(ti => ti.IsGenericType() && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
                //    typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);

                //if (implementsGenericIEnumerableOrIsGenericIEnumerable)
                {
                    type = type.GetGenericArguments () [0];
                }
            }

            var keyProperties = KeyPropertiesCache (type).ToList (); //added ToList() due to issue #418, must work on a list copy
            var explicitKeyProperties = ExplicitKeyPropertiesCache (type);
            if (keyProperties.Count == 0 && explicitKeyProperties.Count == 0) {
                throw new ArgumentException ("Entity must have at least one [Key] or [ExplicitKey] property");
            }

            var name = GetTableName (type);

            var sbSql = new StringBuilder ();
            sbSql.AppendFormat ("update {0} set ", name);

            var allProperties = TypePropertiesCache (type);
            keyProperties.AddRange (explicitKeyProperties);
            var ignoredProperties = IgnoredPropertiesCache (type);
            var nonIdProps = allProperties.Except (keyProperties.Union (ignoredProperties)).ToList ();

            var adapter = GetFormatter (connection);

            for (var i = 0; i < nonIdProps.Count; i++) {
                var property = nonIdProps[i];
                sbSql.Append (adapter.GetColumnMatchesValue (property.GetColumnName (),
                    property.Name, MatchType.Equal)); //fix for issue #336
                if (i < nonIdProps.Count - 1) {
                    sbSql.Append (", ");
                }
            }

            sbSql.Append (" where ");

            for (var i = 0; i < keyProperties.Count; i++) {
                var property = keyProperties[i];
                sbSql.Append (adapter.GetColumnMatchesValue (property.GetColumnName (),
                    property.Name, MatchType.Equal)); //fix for issue #336
                if (i < keyProperties.Count - 1) {
                    sbSql.Append (" and ");
                }
            }

            var updated = connection.Execute (sbSql.ToString (), entityToUpdate, commandTimeout : commandTimeout, transaction : transaction);
            return updated > 0;
        }

        /// <summary>
        /// 根据条件批量更新数据
        /// </summary>
        /// <param name="cmd">更新命令实例</param>
        /// <param name="predicate">WHERE匹配谓词实例</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool UpdateWhere<T> (this UpdateCommand cmd, SqlPredicate<T> predicate)
        where T : class, new () {
            var (sbSql, parameters) = cmd.GetSql (
                predicate,
                getSetSql: (adapter, dyncParms) => GetSetSql<T> (
                    adapter, dyncParms, predicate.Condition, cmd.UpdateFields));

            var updated = cmd.Connection.Execute (sbSql, parameters,
                cmd.Transaction, commandTimeout : cmd.TimeoutSeconds);
            return updated > 0;
        }
    }
}