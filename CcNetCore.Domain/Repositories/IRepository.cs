using System;
using System.Collections.Generic;

namespace CcNetCore.Domain.Repositories {
    /// <summary>
    /// 仓储接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class, new () {
        /// <summary>
        /// 查询所有项列表
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <returns></returns>
        (IEnumerable<T>, Exception) Query (int pageSize, int pageIndex);

        /// <summary>
        /// 根据已有项查询所有匹配的项列表
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <param name="matchFields">匹配的字段列表</param>
        /// <returns></returns>
        (IEnumerable<T>, Exception) Query (T query, params string[] matchFields);

        /// <summary>
        /// 根据已有项查询所有匹配的项列表
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="query">查询实体</param>
        /// <param name="matchFields">匹配的字段列表</param>
        /// <returns></returns>
        (IEnumerable<T>, Exception) Query (int pageSize, int pageIndex, T query, params string[] matchFields);

        /// <summary>
        /// 批量添加项
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Exception Add (IEnumerable<T> items);

        /// <summary>
        /// 更新项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Exception Update (T item);

        /// <summary>
        /// 批量更新所有匹配的项列表
        /// </summary>
        /// <param name="inField">IN匹配的字段名</param>
        /// <param name="inValues">IN匹配的值列表</param>
        /// <param name="item">要更新的值对象</param>
        /// <param name="updateFields">更新的字段列表</param>
        /// <returns></returns>
        Exception UpdateIn (string inField, IEnumerable<object> inValues, T item, params string[] updateFields);

        /// <summary>
        /// 根据已有项删除所有匹配的项列表
        /// </summary>
        /// <param name="item"></param>
        /// <param name="matchFields">匹配的字段列表</param>
        /// <returns></returns>
        Exception Delete (T item, params string[] matchFields);

        /// <summary>
        /// 批量删除所有匹配的项列表
        /// </summary>
        /// <param name="inField">IN匹配的字段名</param>
        /// <param name="inValues">IN匹配的值列表</param>
        /// <returns></returns>
        Exception DeleteIn (string inField, IEnumerable<object> inValues);
    }
}