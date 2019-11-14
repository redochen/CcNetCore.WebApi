using System;
using System.Collections.Generic;
using CcNetCore.Application.Interfaces;
using CcNetCore.Common;
using Newtonsoft.Json;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 结果类
    /// </summary>
    public class Result : IResult {
        /// <summary>
        /// 错误代码
        /// </summary>
        /// <value></value>
        public int Code { get; set; }

        /// <summary>
        /// 相关信息
        /// </summary>
        /// <value></value>
        public string Message { get; set; }

        /// <summary>
        /// 异常对象
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// 结果类
    /// </summary>
    public class Result<T> : Result {
        /// <summary>
        /// 数据对象
        /// </summary>
        /// <value></value>
        public T Data { get; set; }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> GetResult (T data) {
            var result = ErrorCode.Success.ToResult<Result<T>> ();
            result.Data = data;
            return result;
        }
    }

    /// <summary>
    /// 列表结果类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListResult<T> : Result {
        /// <summary>
        /// 数据列表
        /// </summary>
        /// <value></value>
        public List<T> Items { get; set; }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static ListResult<T> GetResult (List<T> items) {
            var result = ErrorCode.Success.ToResult<ListResult<T>> ();
            result.Items = items;
            return result;
        }
    }

    /// <summary>
    /// 分页结果类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageResult<T> : ListResult<T>, IPage {
        /// <summary>
        /// 每页显示记录数（小于或等于0表示不分页显示）
        /// </summary>
        /// <value></value>
        public int PageSize { get; set; }

        /// <summary>
        /// 页码，从1开始
        /// </summary>
        /// <value></value>
        public int PageNo { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        /// <value></value>
        public long TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        /// <value></value>
        public long TotalPages { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PageResult () { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="page"></param>
        public PageResult (IPage page) {
            PageSize = page?.PageSize ?? 0;
            PageNo = page?.PageNo ?? 0;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static PageResult<T> GetResult (List<T> items, IPage page) {
            var result = new PageResult<T> (page);
            result.SetError (ErrorCode.Success);
            result.Items = items;
            return result;
        }
    }

    public static class Results {
        public static readonly Result InvalidParam = "参数错误".ToResult ();
        public static readonly Result InternalError = "内部错误".ToResult ();
        public static readonly Result NeedLogin = "未登录".ToResult ();
        public static readonly Result InvalidIdentity = "账号或密码错误".ToResult ();
        public static readonly Result ForbiddenAccount = "账号已被禁用".ToResult ();
        public static readonly Result LockedAccount = "账号已被锁定".ToResult ();
    }

    public static class ResultExtension {
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="code">错误代码</param>
        /// <returns></returns>
        public static Result ToResult (this ErrorCode code) =>
            code.ToResult<Result> ();

        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <returns></returns>
        public static Result ToResult (this Exception ex) =>
            ex.ToResult<Result> ();

        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public static Result ToResult (this string error) =>
            error.ToResult<Result> ();

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="result"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Result ToResult (this IResult result) =>
            result.ToResult<Result> ();

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="code">错误代码</param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult ToResult<TResult> (this ErrorCode code)
        where TResult : IResult, new () {
            var result = new TResult ();
            result.SetError (code);

            return result;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult ToResult<TResult> (this Exception ex)
        where TResult : IResult, new () {
            var result = new TResult ();
            result.SetError (ex);

            return result;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult ToResult<TResult> (this string error)
        where TResult : IResult, new () {
            var result = new TResult ();
            result.SetError (error);

            return result;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="result"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult ToResult<TResult> (this IResult result)
        where TResult : IResult, new () {
            var res = new TResult ();
            res.SetResult (result);
            return res;
        }
    }
}