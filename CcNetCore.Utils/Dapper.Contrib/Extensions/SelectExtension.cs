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

            var items = connection.Get<T> (sql, dynParms, transaction, commandTimeout);
            return items?.FirstOrDefault ();
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

            return connection.Get<T> (sbSql.ToString (), transaction : transaction, commandTimeout : commandTimeout);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="connection">数据库连接实例</param>
        /// <param name="sql">查询SQL</param>
        /// <param name="parameters">参数列表</param>
        /// <param name="transaction">事务实例</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Get<T> (this IDbConnection connection, string sql, object parameters = null,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new () {
            var result = connection.Query (sql, parameters, transaction, commandTimeout : commandTimeout);

            var items = new List<T> ();
            foreach (IDictionary<string, object> res in result) {
                var obj = DataToEntity<T> (res);
                if (obj != null) {
                    items.Add (obj);
                }
            }

            return items;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="conn">连接实例</param>
        /// <param name="predicate">WHERE匹配谓词实例</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (long Count, IEnumerable<T> Items) GetWhere<T> (this IDbConnection conn, SqlPredicate<T> predicate)
        where T : class, new () =>
            new SelectCommand (conn).GetWhere (predicate);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="cmd">选择命令实例</param>
        /// <param name="predicate">WHERE匹配谓词实例</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (long Count, IEnumerable<T> Items) GetWhere<T> (this SelectCommand cmd, SqlPredicate<T> predicate)
        where T : class, new () {
            var size = (cmd.PageSize ?? 0);
            var index = (cmd.PageIndex ?? 0);
            string sqlNoPage = null;

            var (sql, parameters) = cmd.GetSql (
                predicate,
                getExtWhereSql: (adapter, sbSql) => {
                    sqlNoPage = sbSql.ToString ();

                    if (!cmd.SortFields.IsEmpty ()) {
                        var orderby = cmd.SortFields.Select (x => new SqlOrder (
                            GetColumnName<T> (x.Field), x.Asc));
                        adapter.GetOrderBySql (sbSql, orderby);
                    }

                    adapter.GetPageQuerySql (sbSql, cmd.PageSize, cmd.PageIndex);
                });

            var items = cmd.Connection.Get<T> (sql, parameters, cmd.Transaction, cmd.TimeoutSeconds);
            if (size <= 0 || index < 0) //无分页
            {
                return (items?.Count () ?? 0, items);
            }

            //获取总数
            var selectCount = "select count(*) as count from";
            var getCountSql = sqlNoPage.Replace (cmd.Verb, selectCount);
            var counts = cmd.Connection.Query<CountTable> (getCountSql,
                parameters, cmd.Transaction, commandTimeout : cmd.TimeoutSeconds);

            return (counts?.FirstOrDefault ()?.Count ?? 0, items);
        }

    }
}