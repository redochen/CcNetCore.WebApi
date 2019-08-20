using System;
using Newtonsoft.Json;
using CcNetCore.Common;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 结果基类
    /// </summary>
    public class BaseResult : IResult {
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
    public class Result<T> : BaseResult {
        /// <summary>
        /// 数据对象
        /// </summary>
        /// <value></value>
        public T Data { get; set; }
    }

    public static class ResultExtension {
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="code">错误代码</param>
        /// <returns></returns>
        public static BaseResult ToResult (this ErrorCode code) =>
            code.ToResult<BaseResult> ();

        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <returns></returns>
        public static BaseResult ToResult (this Exception ex) =>
            ex.ToResult<BaseResult> ();

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
    }
}