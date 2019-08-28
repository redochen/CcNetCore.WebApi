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
        /// Delete entity in table "Ts".
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToDelete">Entity to delete</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>true if deleted, false if not found</returns>
        public static bool Delete<T> (this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class {
            if (entityToDelete == null) {
                throw new ArgumentException ("Cannot Delete null Object", nameof (entityToDelete));
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
            keyProperties.AddRange (explicitKeyProperties);

            var sbSql = new StringBuilder ();
            sbSql.AppendFormat ("delete from {0} where ", name);

            var adapter = GetFormatter (connection);

            for (var i = 0; i < keyProperties.Count; i++) {
                var property = keyProperties[i];
                sbSql.Append (adapter.GetColumnNameEqualsValue (property)); //fix for issue #336
                if (i < keyProperties.Count - 1) {
                    sbSql.Append (" and ");
                }
            }

            var deleted = connection.Execute (sbSql.ToString (), entityToDelete, transaction, commandTimeout);
            return deleted > 0;
        }

        /// <summary>
        /// Delete all entities in the table related to the type T.
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>true if deleted, false if none found</returns>
        public static bool DeleteAll<T> (this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class {
            var type = typeof (T);
            var name = GetTableName (type);
            var statement = $"delete from {name}";
            var deleted = connection.Execute (statement, null, transaction, commandTimeout);

            //总是返回成功
            return true;
            //return deleted > 0;
        }

        /// <summary>
        /// 根据条件批量删除数据
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="matchSql">WHERE语句</param>
        /// <param name="parameters">参数列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool DeleteWhere<T> (this IDbConnection connection, IDbTransaction transaction,
            string matchSql, Dictionary<string, object> parameters)
        where T : class, new () =>
            connection.DeleteWhere<T> (transaction, commandTimeout : null, matchSql, parameters);

        /// <summary>
        /// 根据条件批量删除数据
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="matchSql">WHERE语句</param>
        /// <param name="parameters">参数列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool DeleteWhere<T> (this IDbConnection connection, IDbTransaction transaction,
            int? commandTimeout, string matchSql, Dictionary<string, object> parameters)
        where T : class, new () =>
            connection.DeleteWhere (transaction, commandTimeout,
                getWhereSql: (adapter, sbWhere, dyncParms) => GetWhereSql<T> (
                    adapter, sbWhere, dyncParms, matchSql, parameters));

        /// <summary>
        /// 根据条件批量删除数据
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="entityToDelete">要删除的匹配条件</param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="matchFields">匹配字段列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool DeleteWhere<T> (this IDbConnection connection, IDbTransaction transaction,
            T entityToDelete, string matchSql, params string[] matchFields)
        where T : class, new () =>
            connection.DeleteWhere<T> (transaction, commandTimeout : null, entityToDelete, matchSql, matchFields);

        /// <summary>
        /// 根据条件批量删除数据
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="entityToDelete">要删除的匹配条件</param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="matchFields">匹配字段列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool DeleteWhere<T> (this IDbConnection connection, IDbTransaction transaction,
            int? commandTimeout, T entityToDelete, string matchSql, params string[] matchFields)
        where T : class, new () {
            if (entityToDelete == null) {
                throw new ArgumentException ("Cannot Delete null Object", nameof (entityToDelete));
            }

            var opCode = GetOpCode (matchSql);

            return connection.DeleteWhere (transaction, commandTimeout,
                getWhereSql: (adapter, sbWhere, dyncParms) => GetWhereSql (
                    adapter, sbWhere, dyncParms, opCode, entityToDelete, matchSql, autoMatch : true, matchFields));
        }

        /// <summary>
        /// 根据条件批量删除数据
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="getWhereSql">获取WHERE匹配语句的方法</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool DeleteWhere (this IDbConnection connection, IDbTransaction transaction, int? commandTimeout,
            Func<ISqlAdapter /*adapter*/ , StringBuilder /*sbWhere*/ , DynamicParameters /*dyncParms*/ , string> getWhereSql) {
            var (sbSql, parameters) = connection.GetSql (getVerbSql: () => "delete from", getWhereSql : getWhereSql);
            var deleted = connection.Execute (sbSql, parameters, transaction, commandTimeout : commandTimeout);
            return deleted > 0;
        }
    }
}