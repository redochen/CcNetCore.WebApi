using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CcNetCore.Domain.Entities;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;
using Dapper.Contrib.Extensions;

namespace CcNetCore.Infrastructure.Repositories {
    /// <summary>
    /// 实体仓储基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityRepository<T> : DapperRepository, IRepository<T>
        where T : BaseEntity, new () {
            /// <summary>
            /// 构造函数
            /// </summary>
            protected EntityRepository () : base () {
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
            /// 查询所有项列表
            /// </summary>
            /// <param name="pageSize">每页显示数</param>
            /// <param name="pageIndex">页码，从0开始</param>
            /// <returns></returns>
            public virtual (long Count, IEnumerable<T> Items, Exception Exception) Select (
                    int pageSize, int pageIndex) =>
                base.Select<T> (pageSize, pageIndex);

            /// <summary>
            /// 根据已有项查询所有匹配的项列表
            /// </summary>
            /// <param name="condition">查询实体</param>
            /// <param name="matchFields">匹配的字段列表</param>
            /// <returns></returns>
            public virtual (long Count, IEnumerable<T> Items, Exception Exception) Select (
                    T condition, params string[] matchFields) =>
                base.Select<T> (new SqlPredicate<T> (condition, matchFields));

            /// <summary>
            /// 根据已有项查询所有匹配的项列表
            /// </summary>
            /// <param name="pageSize">每页显示数</param>
            /// <param name="pageIndex">页码，从0开始</param>
            /// <param name="condition">查询实体</param>
            /// <param name="matchFields">匹配的字段列表</param>
            /// <returns></returns>
            public virtual (long Count, IEnumerable<T> Items, Exception Exception) Select (
                    int pageSize, int pageIndex, T condition, params string[] matchFields) =>
                base.Select<T> (pageSize, pageIndex, new SqlPredicate<T> (condition, matchFields));

            /// <summary>
            /// 批量添加项
            /// </summary>
            /// <param name="items"></param>
            /// <returns></returns>
            public virtual Exception Add (IEnumerable<T> items) =>
                _Dapper.Add (items, InsertItems);

            /// <summary>
            /// 更新项
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public virtual Exception Update (T item) =>
                _Dapper.Update (item, UpdateItem);

            /// <summary>
            /// 更新项
            /// </summary>
            /// <param name="item"></param>
            /// <param name="updateFields">更新的字段列表</param>
            /// <param name="matchFields">匹配的字段列表</param>
            /// <returns></returns>
            public Exception Update (T item, string[] updateFields, params string[] matchFields) =>
                base.Update (new SqlPredicate<T> (item, matchFields), updateFields);

            /// <summary>
            /// 批量更新所有匹配的项列表
            /// </summary>
            /// <param name="inField">IN匹配的字段名</param>
            /// <param name="inValues">IN匹配的值列表</param>
            /// <param name="item">要更新的值对象</param>
            /// <param name="updateFields">更新的字段列表</param>
            /// <returns></returns>
            public virtual Exception UpdateIn (string inField, IEnumerable<object> inValues,
                    T item, params string[] updateFields) =>
                _Dapper.UpdateIn (item, inField, inValues, updateFields);

            /// <summary>
            /// 根据已有项删除所有匹配的项列表
            /// </summary>
            /// <param name="condition"></param>
            /// <param name="matchFields">匹配的字段列表</param>
            /// <returns></returns>
            public virtual Exception Delete (T condition, params string[] matchFields) =>
                _Dapper.Delete (new SqlPredicate<T> (condition, matchFields));

            /// <summary>
            /// 根据已有项删除所有匹配的项列表
            /// </summary>
            /// <param name="condition"></param>
            /// <param name="getMatchType">获取匹配类型</param>
            /// <param name="matchFields">匹配的字段列表</param>
            /// <returns></returns>
            public virtual Exception Delete (T condition, GetMatchTypeDelegate getMatchType,
                    params string[] matchFields) =>
                _Dapper.Delete (new SqlPredicate<T> (condition, matchFields) {
                    GetMatchType = getMatchType
                });

            /// <summary>
            /// 批量删除所有匹配的项列表
            /// </summary>
            /// <param name="inField">IN匹配的字段名</param>
            /// <param name="inValues">IN匹配的值列表</param>
            /// <returns></returns>
            public virtual Exception DeleteIn (string inField, IEnumerable<object> inValues) =>
                _Dapper.DeleteIn<T> (inField, inValues);

            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="conn"></param>
            /// <param name="trans"></param>
            /// <param name="entities">要添加的数据项列表</param>
            /// <returns></returns>
            protected Exception InsertItems (IDbConnection conn, IDbTransaction trans, IEnumerable<T> entities) {
                if (entities.IsEmpty ()) {
                    return Exceptions.InvalidParam;
                }

                T exists = null;
                Exception ex = null;

                entities.ForEach (x => {
                    (exists, ex) = QueryExists (conn, x, isCreation : true);
                    return (ex != null || exists != null);
                });

                if (ex != null) {
                    return ex;
                }

                if (exists != null) {
                    return Exceptions.AlreadyExists;
                }

                return conn.Insert (entities, trans) > 0 ? null : Exceptions.Failure;
            }

            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="conn"></param>
            /// <param name="trans"></param>
            /// <param name="entity">要更新的数据项</param>
            /// <returns></returns>
            protected Exception UpdateItem (IDbConnection conn, IDbTransaction trans, T entity) {
                var (exists, ex) = QueryExists (conn, entity, isCreation : false);
                if (ex != null) {
                    return ex;
                }

                if (null == exists) {
                    return Exceptions.NotFound;
                }

                UpdateExists (exists, entity);

                //统一字段处理
                exists.Status = entity.Status ?? exists.Status;
                exists.IsDeleted = entity.IsDeleted ?? exists.IsDeleted;
                exists.UpdateUser = entity.UpdateUser;
                exists.UpdateTime = DateTime.Now;

                return conn.Update (exists, trans) ? null : Exceptions.Failure;
            }

            /// <summary>
            /// 查询已存在的数据项
            /// </summary>
            /// <param name="conn"></param>
            /// <param name="entity">要保存的数据项</param>
            /// <param name="isCreation">是否为创建，否则为更新</param>
            /// <returns></returns>
            protected (T exists, Exception ex) QueryExists (IDbConnection conn, T entity, bool isCreation) {
                if (null == entity || !entity.Uid.IsValid ()) {
                    return (null, Exceptions.InvalidParam);
                }

                if (isCreation) {
                    return QueryExists (conn, entity);
                }

                var matchFields = new string[] {
                    nameof (entity.Uid), nameof (entity.IsDeleted)
                };

                var (_, items, ex) = Select (entity, matchFields);
                return (items?.FirstOrDefault (), ex);
            }

            /// <summary>
            /// 查询已存在的数据项
            /// </summary>
            /// <param name="conn"></param>
            /// <param name="entity">要保存的数据项</param>
            /// <returns></returns>
            protected abstract (T, Exception) QueryExists (IDbConnection conn, T entity);

            /// <summary>
            /// 更新已存在的数据项
            /// </summary>
            /// <param name="exists">已存在的数据项</param>
            /// <param name="entity">要保存的数据项</param>
            protected abstract void UpdateExists (T exists, T entity);
        }
}