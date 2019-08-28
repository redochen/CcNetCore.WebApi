using System;
using System.Collections.Generic;
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
                sbSql.Append (adapter.GetColumnNameEqualsValue (property)); //fix for issue #336
                if (i < nonIdProps.Count - 1) {
                    sbSql.Append (", ");
                }
            }

            sbSql.Append (" where ");

            for (var i = 0; i < keyProperties.Count; i++) {
                var property = keyProperties[i];
                sbSql.Append (adapter.GetColumnNameEqualsValue (property)); //fix for issue #336
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
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="entityToUpdate"></param>
        /// <param name="updateFields"></param>
        /// <param name="matchSql"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool UpdateWhere<T> (this IDbConnection connection, IDbTransaction transaction,
            T entityToUpdate, IEnumerable<string> updateFields, string matchSql, Dictionary<string, object> parameters)
        where T : class, new () =>
            connection.UpdateWhere<T> (transaction, null, entityToUpdate, updateFields, matchSql, parameters);

        /// <summary>
        /// 根据条件批量更新数据
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="entityToUpdate"></param>
        /// <param name="updateFields"></param>
        /// <param name="matchSql"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool UpdateWhere<T> (this IDbConnection connection, IDbTransaction transaction, int? commandTimeout,
            T entityToUpdate, IEnumerable<string> updateFields, string matchSql, Dictionary<string, object> parameters)
        where T : class, new () => connection.UpdateWhere<T> (transaction, commandTimeout, entityToUpdate, updateFields,
            getWhereSql: (adapter, sbWhere, dyncParms) => GetWhereSql<T> (
                adapter, sbWhere, dyncParms, matchSql, parameters));

        /// <summary>
        /// 根据条件批量更新数据
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="entityToUpdate"></param>
        /// <param name="updateFields"></param>
        /// <param name="matchSql"></param>
        /// <param name="matchFields"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool UpdateWhere<T> (this IDbConnection connection, IDbTransaction transaction,
            T entityToUpdate, IEnumerable<string> updateFields, string matchSql, params string[] matchFields)
        where T : class, new () =>
            connection.UpdateWhere<T> (transaction, null, entityToUpdate, updateFields, matchSql, matchFields);

        /// <summary>
        /// 根据条件批量更新数据
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="entityToUpdate"></param>
        /// <param name="updateFields"></param>
        /// <param name="matchSql"></param>
        /// <param name="matchFields"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool UpdateWhere<T> (this IDbConnection connection, IDbTransaction transaction, int? commandTimeout,
            T entityToUpdate, IEnumerable<string> updateFields, string matchSql, params string[] matchFields)
        where T : class, new () {
            if (entityToUpdate == null) {
                throw new ArgumentException ("Cannot Update null Object", nameof (entityToUpdate));
            }

            var opCode = GetOpCode (matchSql);

            return connection.UpdateWhere<T> (transaction, commandTimeout, entityToUpdate, updateFields,
                getWhereSql: (adapter, sbWhere, dyncParms) => GetWhereSql (
                    adapter, sbWhere, dyncParms, opCode, entityToUpdate, matchSql, autoMatch : true, matchFields));
        }

        /// <summary>
        /// 根据条件批量更新数据
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="entityToUpdate"></param>
        /// <param name="updateFields"></param>
        /// <param name="getWhereSql">获取WHERE匹配语句的方法</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool UpdateWhere<T> (this IDbConnection connection, IDbTransaction transaction,
            int? commandTimeout, T entityToUpdate, IEnumerable<string> updateFields,
            Func<ISqlAdapter /*adapter*/ , StringBuilder /*sbWhere*/ , DynamicParameters /*dyncParms*/ , string> getWhereSql)
        where T : class, new () {
            var (sbSql, parameters) = connection.GetSql (
                getVerbSql: () => "update",
                getWhereSql : getWhereSql,
                getSetSql: (adapter, dyncParms) => GetSetSql<T> (
                    adapter, dyncParms, entityToUpdate, updateFields));

            var updated = connection.Execute (sbSql, parameters, transaction, commandTimeout : commandTimeout);
            return updated > 0;
        }
    }
}