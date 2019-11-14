using System;
using System.Collections.Generic;
using System.Data;
using CcNetCore.Utils.Helpers;
using Dapper.Contrib.Extensions;

namespace CcNetCore.Infrastructure.Repositories {
    /// <summary>
    /// Dapper仓储基类
    /// </summary>
    public abstract class DapperRepository : IDbProvider {
        protected DapperHelper _Dapper = null;

        /// <summary>
        /// 自动装载属性（必须为public，否则自动装载失败）
        /// </summary>
        /// <value></value>
        public IDbProvider _Provider { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DapperRepository () {
            _Dapper = new DapperHelper (this);
        }

        /// <summary>
        /// 获取数据库连接实例
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection () =>
            _Provider?.GetConnection ();

        /// <summary>
        /// 查询所有项
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual (long Count, IEnumerable<T> Items, Exception Exception) Select<T> (
                int pageSize, int pageIndex) where T : class, new () =>
            _Dapper.Select<T> (pageSize, pageIndex);

        /// <summary>
        /// 根据已有项查询所有匹配的项列表
        /// </summary>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual (long Count, IEnumerable<T> Items, Exception Exception) Select<T> (
                SqlPredicate<T> predicate) where T : class, new () =>
            _Dapper.Select (predicate);

        /// <summary>
        /// 根据已有项查询所有匹配的项列表
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="predicate">查询实体</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual (long Count, IEnumerable<T> Items, Exception Exception) Select<T> (
                int pageSize, int pageIndex, SqlPredicate<T> predicate) where T : class, new () =>
            _Dapper.Select (pageSize, pageIndex, predicate);

        /// <summary>
        /// 执行查询SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual (long Count, IEnumerable<T> Items, Exception Exception) Select<T> (
                string sql, object param = null) where T : class, new () =>
            _Dapper.Select<T> (sql, param);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateFields">要更新的字段</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual Exception Update<T> (SqlPredicate<T> predicate, params string[] updateFields)
        where T : class, new () =>
            _Dapper.Update<T> (predicate, updateFields);

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual Exception Execute (string sql, object param = null) =>
            _Dapper.Execute (sql, param);

        /// <summary>
        /// 根据已有项删除所有匹配的项列表
        /// </summary>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual Exception Delete<T> (SqlPredicate<T> predicate) where T : class, new () =>
            _Dapper.Delete (predicate);
    }
}