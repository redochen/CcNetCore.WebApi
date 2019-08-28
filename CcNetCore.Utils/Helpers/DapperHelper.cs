#pragma warning disable CS0168

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper.Contrib.Extensions;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.Utils.Helpers {
    /// <summary>
    /// 数据库提供方接口
    /// </summary>
    public interface IDbProvider {
        IDbConnection GetConnection ();
    }

    /// <summary>
    /// Dapper帮助类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DapperHelper<TEntity> where TEntity : class, new () {
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
        /// <returns></returns>
        public (IEnumerable<TEntity>, Exception) Query (int pageSize, int pageIndex) {
            return Query (conn => {
                var data = conn.GetWhere<TEntity> (pageSize, pageIndex);
                return (data?.ToList (), null);
            });
        }

        /// <summary>
        /// 根据已有实体查询匹配的实体列表
        /// </summary>
        /// <param name="entityToQuery"></param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="matchFields">匹配的字段列表</param>
        /// <returns></returns>
        public (IEnumerable<TEntity>, Exception) Query (TEntity entityToQuery, string matchSql, params string[] matchFields) {
            return Query (conn => {
                var data = conn.GetWhere<TEntity> (entityToQuery, matchSql, matchFields);
                return (data?.ToList (), null);
            });
        }

        /// <summary>
        /// 根据已有实体查询匹配的实体列表
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="entityToQuery"></param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="matchFields">匹配的字段列表</param>
        /// <returns></returns>
        public (IEnumerable<TEntity>, Exception) Query (int pageSize, int pageIndex,
            TEntity entityToQuery, string matchSql, params string[] matchFields) {
            return Query (conn => {
                var data = conn.GetWhere<TEntity> (pageSize, pageIndex, entityToQuery, matchSql, matchFields);
                return (data?.ToList (), null);
            });
        }

        /// <summary>
        /// 添加实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="insertFunc"></param>
        /// <returns></returns>
        public Exception Add (IEnumerable<TEntity> entities,
            Func<IDbConnection, IDbTransaction, IEnumerable<TEntity>, Exception> insertFunc) {
            if (entities.IsEmpty ()) {
                return Exceptions.InvalidParam;
            }

            return Execute ((conn, trans) => insertFunc (conn, trans, entities));
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <param name="updateFunc"></param>
        /// <returns></returns>
        public Exception Update (TEntity entityToUpdate,
            Func<IDbConnection, IDbTransaction, TEntity, Exception> updateFunc) {
            if (null == entityToUpdate) {
                return Exceptions.InvalidParam;
            }

            return Execute ((conn, trans) => updateFunc (conn, trans, entityToUpdate));
        }

        /// <summary>
        /// 根据已有实体删除匹配的实体列表
        /// </summary>
        /// <param name="matchSql">匹配的SQL</param>
        /// <param name="parameters">参数值</param>
        /// <param name="entityToUpdate">更新的值对象</param>
        /// <param name="updateFields">更新的字段列表</param>
        /// <returns></returns>
        public Exception Update (string matchSql, Dictionary<string, object> parameters,
            TEntity entityToUpdate, params string[] updateFields) {
            if (null == entityToUpdate || !matchSql.IsValid ()) {
                return Exceptions.InvalidParam;
            }

            return Execute ((conn, trans) => {
                var success = conn.UpdateWhere<TEntity> (trans, entityToUpdate, updateFields, matchSql, parameters);
                return success ? null : Exceptions.Failure;
            });
        }

        /// <summary>
        /// 根据已有实体删除匹配的实体列表
        /// </summary>
        /// <param name="entityToDelete"></param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <param name="matchFields">匹配的字段列表</param>
        /// <returns></returns>
        public Exception Delete (TEntity entityToDelete, string matchSql, params string[] matchFields) {
            if (null == entityToDelete || !matchSql.IsValid ()) {
                return Exceptions.InvalidParam;
            }

            return Execute ((conn, trans) => {
                var success = conn.DeleteWhere<TEntity> (trans, entityToDelete, matchSql, matchFields);
                return success ? null : Exceptions.Failure;
            });
        }

        /// <summary>
        /// 根据已有实体删除匹配的实体列表
        /// </summary>
        /// <param name="matchSql">匹配的SQL</param>
        /// <param name="parameters">参数值</param>
        /// <returns></returns>
        public Exception Delete (string matchSql, Dictionary<string, object> parameters) {
            if (!matchSql.IsValid ()) {
                return Exceptions.InvalidParam;
            }

            return Execute ((conn, trans) => {
                var success = conn.DeleteWhere<TEntity> (trans, matchSql, parameters);
                return success ? null : Exceptions.Failure;
            });
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="queryFunc">查询方法</param>
        /// <returns></returns>
        private (IEnumerable<TEntity>, Exception) Query (Func < IDbConnection, (IEnumerable<TEntity>, Exception) > queryFunc) {
            using (var conn = _DbProvider.GetConnection ()) {
                conn.Open ();

                try {
                    return queryFunc (conn);
                } catch (Exception ex) {
                    return (null, ex);
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