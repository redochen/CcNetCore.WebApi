using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using CcNetCore.Domain.Repositories;
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
        /// <param name="matchFields">匹配的字段列表</param>
        /// <returns></returns>
        public (IEnumerable<T>, Exception) Query (T query, params string[] matchFields) =>
            _Dapper.Query (query, MatchSql.AND, matchFields);

        /// <summary>
        /// 根据已有项查询所有匹配的项列表
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="query">查询实体</param>
        /// <param name="matchFields">匹配的字段列表</param>
        /// <returns></returns>
        public (IEnumerable<T>, Exception) Query (int pageSize, int pageIndex, T query, params string[] matchFields) =>
            _Dapper.Query (pageSize, pageIndex, query, MatchSql.AND, matchFields);

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
        /// 批量更新所有匹配的项列表
        /// </summary>
        /// <param name="inField">IN匹配的字段名</param>
        /// <param name="inValues">IN匹配的值列表</param>
        /// <param name="item">要更新的值对象</param>
        /// <param name="updateFields">更新的字段列表</param>
        /// <returns></returns>
        public Exception UpdateIn (string inField, IEnumerable<object> inValues, T item, params string[] updateFields) {
            var (matchSql, parameters) = GetInMatch (inField, inValues);
            return _Dapper.Update (matchSql, parameters, item, updateFields);
        }

        /// <summary>
        /// 根据已有项删除所有匹配的项列表
        /// </summary>
        /// <param name="item"></param>
        /// <param name="matchFields">匹配的字段列表</param>
        /// <returns></returns>
        public Exception Delete (T item, params string[] matchFields) =>
            _Dapper.Delete (item, MatchSql.AND, matchFields);

        /// <summary>
        /// 批量删除所有匹配的项列表
        /// </summary>
        /// <param name="inField">IN匹配的字段名</param>
        /// <param name="inValues">IN匹配的值列表</param>
        /// <returns></returns>
        public Exception DeleteIn (string inField, IEnumerable<object> inValues) {
            var (matchSql, parameters) = GetInMatch (inField, inValues);
            return _Dapper.Delete (matchSql, parameters);
        }

        /// <summary>
        /// 获取IN匹配SQL
        /// </summary>
        /// <param name="inField">IN匹配的字段名</param>
        /// <param name="inValues">IN匹配的值列表</param>
        /// <returns></returns>
        private (string, Dictionary<string, object>) GetInMatch (string inField, IEnumerable<object> inValues) {
            var matchSql = "{0} in {1}";
            var parameters = new Dictionary<string, object> {
                    [inField] = inValues?.ToArray (),
                };

            return (matchSql, parameters);
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