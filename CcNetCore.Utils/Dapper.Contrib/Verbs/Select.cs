using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CcNetCore.Utils.Extensions;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// The Dapper.Contrib extensions for Dapper
    /// </summary>
    public static partial class SqlMapperExtensions {
        /// <summary>
        /// Returns a single entity by a single id from table "Ts".
        /// Id must be marked with [Key] attribute.
        /// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
        /// for optimal performance.
        /// </summary>
        /// <typeparam name="T">Interface or type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="id">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>Entity of T</returns>
        public static T Get<T> (this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null)
        where T : class, new () {
            var type = typeof (T);

            if (!GetQueries.TryGetValue (type.TypeHandle, out string sql)) {
                var key = GetSingleKey<T> (nameof (Get));
                var name = GetTableName (type);

                sql = $"select * from {name} where {key.Name} = @id";
                GetQueries[type.TypeHandle] = sql;
            }

            var dynParms = new DynamicParameters ();
            dynParms.Add ("@id", id);

            if (!(connection.Query (sql, dynParms, transaction : transaction, commandTimeout : commandTimeout)
                    .FirstOrDefault () is IDictionary<string, object> res)) {
                return null;
            }

            return DataToEntity<T> (res);
        }

        /// <summary>
        /// Returns a list of entites from table "Ts".
        /// Id of T must be marked with [Key] attribute.
        /// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
        /// for optimal performance.
        /// </summary>
        /// <typeparam name="T">Interface or type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <returns>Entity of T</returns>
        public static IEnumerable<T> GetAll<T> (this IDbConnection connection)
        where T : class, new () =>
            connection.GetAll<T> (null, null, null, null);

        /// <summary>
        /// Returns a list of entites from table "Ts".
        /// Id of T must be marked with [Key] attribute.
        /// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
        /// for optimal performance.
        /// </summary>
        /// <typeparam name="T">Interface or type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>Entity of T</returns>
        public static IEnumerable<T> GetAll<T> (this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null)
        where T : class, new () =>
            connection.GetAll<T> (transaction, commandTimeout, null, null);

        /// <summary>
        /// Returns a list of entites from table "Ts".
        /// Id of T must be marked with [Key] attribute.
        /// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
        /// for optimal performance.
        /// </summary>
        /// <typeparam name="T">Interface or type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <returns>Entity of T</returns>
        public static IEnumerable<T> GetAll<T> (this IDbConnection connection,
            IDbTransaction transaction = null, int? commandTimeout = null, int? pageSize = null, int? pageIndex = null)
        where T : class, new () {
            var type = typeof (T);
            var cacheType = typeof (List<T>);

            if (!GetQueries.TryGetValue (cacheType.TypeHandle, out string sql)) {
                //GetSingleKey<T> (nameof (GetAll));
                var name = GetTableName (type);

                sql = $"select * from {name}";
                GetQueries[cacheType.TypeHandle] = sql;
            }

            var sbSql = new StringBuilder ();
            var adapter = GetFormatter (connection);
            adapter.GetPageQuerySql (sbSql, pageSize, pageIndex);

            var list = new List<T> ();
            var result = connection.Query (sbSql.ToString (), transaction : transaction, commandTimeout : commandTimeout);

            foreach (IDictionary<string, object> res in result) {
                var obj = DataToEntity<T> (res);
                if (obj != null) {
                    list.Add (obj);
                }
            }

            return list;
        }

        /// <summary>
        /// 无分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="matchSql">WHERE语句</param>
        /// <param name="parameters">参数列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection,
            string matchSql, Dictionary<string, object> parameters)
        where T : class, new () =>
            connection.GetWhere<T> (transaction: null, commandTimeout: null,
                pageSize: null, pageIndex: null, matchSql, parameters);

        /// <summary>
        /// 无分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="matchSql">WHERE语句</param>
        /// <param name="parameters">参数列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection,
            IDbTransaction transaction, int? commandTimeout, string matchSql,
            Dictionary<string, object> parameters)
        where T : class, new () =>
            connection.GetWhere<T> (transaction, commandTimeout,
                pageSize : null, pageIndex : null, matchSql, parameters);

        /// <summary>
        /// 带分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection, int? pageSize, int? pageIndex)
        where T : class, new () =>
            connection.GetWhere<T> (transaction: null, commandTimeout: null,
                pageSize, pageIndex, matchSql : null, parameters : null);

        /// <summary>
        /// 带分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="matchSql">WHERE语句</param>
        /// <param name="parameters">参数列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection,
            int? pageSize, int? pageIndex, string matchSql, Dictionary<string, object> parameters)
        where T : class, new () =>
            connection.GetWhere<T> (transaction: null, commandTimeout: null,
                pageSize, pageIndex, matchSql, parameters);

        /// <summary>
        /// 带分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="matchSql">WHERE语句</param>
        /// <param name="parameters">参数列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection,
            IDbTransaction transaction, int? commandTimeout, int? pageSize, int? pageIndex,
            string matchSql, Dictionary<string, object> parameters)
        where T : class, new () =>
            connection.GetWhere<T> (transaction, commandTimeout, pageSize, pageIndex,
                getWhereSql: (adapter, sbWhere, dyncParms) => GetWhereSql<T> (
                    adapter, sbWhere, dyncParms, matchSql, parameters));

        /// <summary>
        /// 无分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="query">查询条件对象</param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="matchFields">匹配字段列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection,
            T query, string matchSql, params string[] matchFields)
        where T : class, new () =>
            connection.GetWhere<T> (transaction: null, commandTimeout: null,
                pageSize: null, pageIndex: null, query, matchSql, autoMatch : true, matchFields);

        /// <summary>
        /// 无分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="query">查询条件对象</param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="autoMatch">如果matchFields为空，是否自动识别匹配字段</param>
        /// <param name="matchFields">匹配字段列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection,
            T query, string matchSql, bool autoMatch, params string[] matchFields)
        where T : class, new () =>
            connection.GetWhere<T> (transaction: null, commandTimeout: null,
                pageSize: null, pageIndex: null, query, matchSql, autoMatch, matchFields);

        /// <summary>
        /// 带分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="query">查询条件对象</param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="matchFields">匹配字段列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection, int? pageSize,
            int? pageIndex, T query, string matchSql, params string[] matchFields)
        where T : class, new () =>
            connection.GetWhere<T> (transaction: null, commandTimeout: null,
                pageSize, pageIndex, query, matchSql, autoMatch : true, matchFields);

        /// <summary>
        /// 带分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="query">查询条件对象</param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="autoMatch">如果matchFields为空，是否自动识别匹配字段</param>
        /// <param name="matchFields">匹配字段列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection, int? pageSize,
            int? pageIndex, T query, string matchSql, bool autoMatch, params string[] matchFields)
        where T : class, new () =>
            connection.GetWhere<T> (transaction: null, commandTimeout: null,
                pageSize, pageIndex, query, matchSql, autoMatch, matchFields);

        /// <summary>
        /// 无分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="query">查询条件对象</param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="matchFields">匹配字段列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection,
            IDbTransaction transaction, int? commandTimeout, T query, string matchSql,
            params string[] matchFields)
        where T : class, new () =>
            connection.GetWhere<T> (transaction, commandTimeout, pageSize : null,
                pageIndex : null, query, matchSql, autoMatch : true, matchFields);

        /// <summary>
        /// 无分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="query">查询条件对象</param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="autoMatch">如果matchFields为空，是否自动识别匹配字段</param>
        /// <param name="matchFields">匹配字段列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection,
            IDbTransaction transaction, int? commandTimeout, T query, string matchSql,
            bool autoMatch, params string[] matchFields)
        where T : class, new () =>
            connection.GetWhere<T> (transaction, commandTimeout, pageSize : null,
                pageIndex : null, query, matchSql, autoMatch, matchFields);

        /// <summary>
        /// 带分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="query">查询条件对象</param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="autoMatch">如果matchFields为空，是否自动识别匹配字段</param>
        /// <param name="matchFields">匹配字段列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetWhere<T> (this IDbConnection connection,
            IDbTransaction transaction, int? commandTimeout, int? pageSize, int? pageIndex,
            T query, string matchSql, bool autoMatch, params string[] matchFields)
        where T : class, new () {
            if (null == query) {
                throw new ArgumentException ("Cannot Query null Object", nameof (query));
            }

            var opCode = GetOpCode (matchSql);

            return connection.GetWhere<T> (transaction, commandTimeout, pageSize, pageIndex,
                getWhereSql: (adapter, sbWhere, dyncParms) => GetWhereSql (
                    adapter, sbWhere, dyncParms, opCode, query, matchSql, autoMatch, matchFields));
        }

        /// <summary>
        /// 带分页查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="getWhereSql">获取WHERE匹配语句的方法</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static IEnumerable<T> GetWhere<T> (this IDbConnection connection,
            IDbTransaction transaction, int? commandTimeout, int? pageSize, int? pageIndex,
            Func<ISqlAdapter /*adapter*/ , StringBuilder /*sbWhere*/ , DynamicParameters /*dyncParms*/ , string> getWhereSql)
        where T : class, new () {
            var (sbSql, parameters) = connection.GetSql (getVerbSql: () => "select * from", getWhereSql : getWhereSql,
                getExtWhereSql: (adapter, sb) => adapter.GetPageQuerySql (sb, pageSize, pageIndex));
            var result = connection.Query (sbSql, parameters, transaction, commandTimeout : commandTimeout);

            var list = new List<T> ();
            foreach (IDictionary<string, object> res in result) {
                var obj = DataToEntity<T> (res);
                if (obj != null) {
                    list.Add (obj);
                }
            }
            return list;
        }

    }
}