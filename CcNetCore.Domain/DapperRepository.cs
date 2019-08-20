using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils;
using CcNetCore.Utils.Helpers;

namespace CcNetCore.Domain {
    /// <summary>
    /// 仓储基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DapperRepository<T> : IDbProvider, IRepository<T> where T : class, new () {
        protected DapperHelper<T> _Dapper = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        protected DapperRepository () {
            _Dapper = new DapperHelper<T> (this);

            Task.Run (() => {
                var hasCreated = false;

                do {
                    try {
                        _Dapper.Execute ((conn, trans) => {
                            hasCreated = conn.CreateIfNotExists<T> ();
                            return null;
                        });
                    } catch { }

                    Thread.Sleep (100);
                }
                while (!hasCreated);
            });
        }

        /// <summary>
        /// 获取连接实例
        /// </summary>
        /// <returns></returns>
        public abstract IDbConnection GetConnection ();

        /// <summary>
        /// 查询所有项
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <returns></returns>
        public (IEnumerable<T>, Exception) Query (int pageSize, int pageIndex) =>
            _Dapper.Query (pageSize, pageIndex);

        /// <summary>
        /// 根据已有项查询所有匹配的项列表
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <returns></returns>
        public (IEnumerable<T>, Exception) Query (T query) =>
            _Dapper.Query (query, MatchSql.AND);

        /// <summary>
        /// 根据已有项查询所有匹配的项列表
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="query">查询实体</param>
        /// <param name="matchSql">WHERE语句或AND或OR</param>
        /// <returns></returns>
        public (IEnumerable<T>, Exception) Query (int pageSize, int pageIndex, T query) =>
            _Dapper.Query (pageSize, pageIndex, query, MatchSql.AND);

        /// <summary>
        /// 批量添加项
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public Exception Add (IEnumerable<T> items) => _Dapper.Add (items, InsertItems);

        /// <summary>
        /// 更新项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Exception Update (T item) => _Dapper.Update (item, UpdateItem);

        /// <summary>
        /// 根据已有项删除所有匹配的项列表
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Exception Delete (T item) => _Dapper.Delete (item, MatchSql.AND);

        /// <summary>
        /// 删除所有匹配的项列表
        /// </summary>
        /// <param name="inField">批量删除时IN匹配的字段名</param>
        /// <param name="inValues"></param>
        /// <returns></returns>
        public Exception Delete (string inField, IEnumerable<object> inValues) {
            var matclSql = "{0} in {1}";
            var parameters = new Dictionary<string, object> {
                    [inField] = inValues?.ToArray (),
                };

            return _Dapper.Delete (matclSql, parameters);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="items">要添加的数据项列表</param>
        /// <returns></returns>
        protected abstract Exception InsertItems (IDbConnection conn, IDbTransaction trans, IEnumerable<T> items);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="item">要更新的数据项</param>
        /// <returns></returns>
        protected abstract Exception UpdateItem (IDbConnection conn, IDbTransaction trans, T item);
    }
}