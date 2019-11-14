#pragma warning disable CS0168

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CcNetCore.Utils.Extensions;
using Dapper;
using Dapper.Contrib.Extensions;

namespace CcNetCore.Utils.Helpers {
    /// <summary>
    /// 数据库提供方接口
    /// </summary>
    public interface IDbProvider {
        /// <summary>
        /// 获取数据库连接实例
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection ();
    }

    /// <summary>
    /// Dapper帮助类
    /// </summary>
    public class DapperHelper {
        private IDbProvider _DbProvider = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbProvider"></param>
        public DapperHelper (IDbProvider dbProvider) {
            _DbProvider = dbProvider;
        }

        /// <summary>
        /// 根据已有实体查询匹配的实体列表
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public (long Count, IEnumerable<T> Items, Exception Exception) Select<T> (
            int pageSize, int pageIndex) where T : class, new () {
            return Query<T> (conn => {
                var cmd = new SelectCommand (conn)
                    .SetPageInfo (pageSize, pageIndex);

                var (count, items) = cmd.GetWhere<T> (predicate: null);
                return (count, items, null);
            });
        }

        /// <summary>
        /// 根据已有实体查询匹配的实体列表
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="orderby">排序字段集合</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public (long Count, IEnumerable<T> Items, Exception Exception) Select<T> (
            int pageSize, int pageIndex, SqlOrder[] orderby) where T : class, new () {
            return Query<T> (conn => {
                var cmd = new SelectCommand (conn) {
                    SortFields = orderby
                }.SetPageInfo (pageSize, pageIndex);

                var (count, items) = cmd.GetWhere<T> (predicate: null);
                return (count, items, null);
            });
        }

        /// <summary>
        /// 根据已有实体查询匹配的实体列表
        /// </summary>
        /// <param name="predicate">WHERE匹配谓词实例</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public (long Count, IEnumerable<T> Items, Exception Exception) Select<T> (
            SqlPredicate<T> predicate) where T : class, new () {
            return Query<T> (conn => {
                var (count, items) = conn.GetWhere<T> (predicate);
                return (count, items, null);
            });
        }

        /// <summary>
        /// 根据已有实体查询匹配的实体列表
        /// </summary>
        /// <param name="predicate">WHERE匹配谓词实例</param>
        /// <param name="orderby">排序字段集合</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public (long Count, IEnumerable<T> Items, Exception Exception) Select<T> (
            SqlPredicate<T> predicate, SqlOrder[] orderby) where T : class, new () {
            return Query<T> (conn => {
                var cmd = new SelectCommand (conn) {
                SortFields = orderby
                };

                var (count, items) = cmd.GetWhere<T> (predicate);
                return (count, items, null);
            });
        }

        /// <summary>
        /// 根据已有实体查询匹配的实体列表
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="predicate">WHERE匹配谓词实例</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public (long Count, IEnumerable<T> Items, Exception Exception) Select<T> (
            int pageSize, int pageIndex, SqlPredicate<T> predicate)
        where T : class, new () {
            return Query<T> (conn => {
                var cmd = new SelectCommand (conn)
                    .SetPageInfo (pageSize, pageIndex);

                var (count, items) = cmd.GetWhere<T> (predicate);
                return (count, items, null);
            });
        }

        /// <summary>
        /// 根据已有实体查询匹配的实体列表
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="predicate">WHERE匹配谓词实例</param>
        /// <param name="orderby">排序字段集合</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public (long Count, IEnumerable<T> Items, Exception Exception) Select<T> (
            int pageSize, int pageIndex, SqlPredicate<T> predicate, SqlOrder[] orderby)
        where T : class, new () {
            return Query<T> (conn => {
                var cmd = new SelectCommand (conn) {
                    SortFields = orderby
                }.SetPageInfo (pageSize, pageIndex);

                var (count, items) = cmd.GetWhere<T> (predicate);
                return (count, items, null);
            });
        }

        /// <summary>
        /// 执行查询SQL
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public (long Count, IEnumerable<T> Items, Exception Exception) Select<T> (
                string sql, object param = null) where T : class, new () =>
            Query<T> (conn => {
                var items = conn.Get<T> (sql, param);
                return (items?.Count () ?? 0, items, null);
            });

        /// <summary>
        /// 添加实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="insertFunc"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Exception Add<T> (IEnumerable<T> entities,
            Func<IDbConnection, IDbTransaction, IEnumerable<T>, Exception> insertFunc) {
            if (entities.IsEmpty ()) {
                return Exceptions.InvalidParam;
            }

            return Execute ((conn, trans) => insertFunc (conn, trans, entities));
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="updateFunc"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Exception Update<T> (T condition, Func<IDbConnection, IDbTransaction, T, Exception> updateFunc)
        where T : class, new () {
            if (null == condition) {
                return Exceptions.InvalidParam;
            }

            return Execute ((conn, trans) => updateFunc (conn, trans, condition));
        }

        /// <summary>
        /// 根据已有实体删除匹配的实体列表
        /// </summary>
        /// <param name="predicate">WHERE匹配谓词实例</param>
        /// <param name="updateFields">更新的字段列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Exception Update<T> (SqlPredicate<T> predicate, params string[] updateFields)
        where T : class, new () {
            if (null == predicate) {
                return Exceptions.InvalidParam;
            }

            return Execute ((conn, trans) => {
                var cmd = new UpdateCommand (conn, trans) {
                UpdateFields = updateFields
                };
                return cmd.UpdateWhere<T> (predicate) ? null : Exceptions.Failure;
            });
        }

        /// <summary>
        /// 批量更新所有匹配的项列表
        /// </summary>
        /// <param name="item">要更新的值对象</param>
        /// <param name="inField">IN匹配的字段名</param>
        /// <param name="inValues">IN匹配的值列表</param>
        /// <param name="updateFields">更新的字段列表</param>
        /// <returns></returns>
        public virtual Exception UpdateIn<T> (T condition, string inField,
            IEnumerable<object> inValues, params string[] updateFields) where T : class, new () {
            if (null == condition || !inField.IsValid () || inValues.IsEmpty ()) {
                return Exceptions.InvalidParam;
            }

            return Execute ((conn, trans) => {
                var cmd = new UpdateCommand (conn, trans) {
                UpdateFields = updateFields
                };

                var (matchSql, parameters) = GetInMatch<T> (conn, inField, inValues);
                var predicate = new SqlPredicate<T> (condition, matchSql, parameters);

                return cmd.UpdateWhere<T> (predicate) ? null : Exceptions.Failure;
            });
        }

        /// <summary>
        /// 根据已有实体删除匹配的实体列表
        /// </summary>
        /// <param name="predicate">WHERE匹配谓词实例</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Exception Delete<T> (SqlPredicate<T> predicate) where T : class, new () {
            if (null == predicate) {
                return Exceptions.InvalidParam;
            }

            return Execute ((conn, trans) => {
                var cmd = new DeleteCommand (conn, trans);
                return cmd.DeleteWhere<T> (predicate) ? null : Exceptions.Failure;
            });
        }

        /// <summary>
        /// 批量删除所有匹配的项列表
        /// </summary>
        /// <param name="inField">IN匹配的字段名</param>
        /// <param name="inValues">IN匹配的值列表</param>
        /// <returns></returns>
        public virtual Exception DeleteIn<T> (string inField, IEnumerable<object> inValues)
        where T : class, new () {
            if (!inField.IsValid () || inValues.IsEmpty ()) {
                return Exceptions.InvalidParam;
            }

            return Execute ((conn, trans) => {
                var cmd = new DeleteCommand (conn, trans);
                var (matchSql, parameters) = GetInMatch<T> (conn, inField, inValues);
                var predicate = new SqlPredicate<T> (matchSql, parameters);

                return cmd.DeleteWhere<T> (predicate) ? null : Exceptions.Failure;
            });
        }

        /// <summary>
        /// 获取IN匹配SQL
        /// </summary>
        /// <param name="inField">IN匹配的字段名</param>
        /// <param name="inValues">IN匹配的值列表</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private (string, Dictionary<string, object>) GetInMatch<T> (IDbConnection conn,
            string inField, IEnumerable<object> inValues) {
            //var matchSql = "{0} in {1}";
            var matchSql = conn.GetMatchExpression<T> (inField, MatchType.In);

            var parameters = new Dictionary<string, object> {
                    [inField] = inValues?.ToArray (),
                };

            return (matchSql, parameters);
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Exception Execute (string sql, object param = null) =>
            Execute ((conn, trans) => {
                conn.Execute (sql, param, trans);
                return null;
            });

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="queryFunc">查询方法</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private (long Count, IEnumerable<T> Items, Exception Exception) Query<T> (
            Func < IDbConnection, (long, IEnumerable<T>, Exception) > queryFunc)
        where T : class, new () {
            using (var conn = _DbProvider.GetConnection ()) {
                conn.Open ();

                try {
                    return queryFunc (conn);
                } catch (Exception ex) {
                    return (0, null, ex);
                } finally {
                    conn.Close ();
                }
            }
        }

        /// <summary>
        /// 执行（事务性）
        /// </summary>
        /// <param name="executeFunc">执行方法</param>
        /// <returns>异常信息</returns>
        public Exception Execute (Func<IDbConnection, IDbTransaction, Exception> executeFunc) {
            Exception exception = null;

            using (var conn = _DbProvider.GetConnection ()) {
                conn.Open ();
                var trans = conn.BeginTransaction ();

                try {
                    exception = executeFunc (conn, trans);
                    trans.Commit ();
                } catch (Exception ex) {
                    exception = ex;
                    trans.Rollback ();
                } finally {
                    conn.Close ();
                }
            }

            return exception;
        }
    }
}