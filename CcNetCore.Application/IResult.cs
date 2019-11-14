using System;
using CcNetCore.Common;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.Application {
    /// <summary>
    /// 结果接口
    /// </summary>
    public interface IResult {
        /// <summary>
        /// 错误代码
        /// </summary>
        /// <value></value>
        int Code { get; set; }

        /// <summary>
        /// 相关信息
        /// </summary>
        /// <value></value>
        string Message { get; set; }

        /// <summary>
        /// 异常对象
        /// </summary>
        /// <value></value>
        Exception Exception { get; set; }
    }

    /// <summary>
    /// IResult扩展类
    /// </summary>
    public static class IResultExtension {
        /// <summary>
        /// 是否成功
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsSuccess (this IResult result) =>
            result.Code == (int) ErrorCode.Success;

        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="result"></param>
        /// <param name="code">错误代码</param>
        public static void SetError (this IResult result, ErrorCode code) {
            result.Code = (int) code;
            result.Message = code.GetDesc ();
            result.Exception = null;
        }

        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="result"></param>
        /// <param name="ex">异常对象</param>
        public static void SetError (this IResult result, Exception ex) {
            if (ex != null) {
                result.SetError (ErrorCode.Failed);
                result.Message = ex.Message;
                result.Exception = ex;
            } else {
                result.SetError (ErrorCode.Success);
            }
        }

        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="result"></param>
        /// <param name="error">错误信息</param>
        public static void SetError (this IResult result, string error) {
            result.SetError (ErrorCode.Failed);
            if (error.IsValid ()) {
                result.Message = error;
            }
        }

        /// <summary>
        /// 设置结果
        /// </summary>
        /// <param name="result"></param>
        /// <param name="res"></param>
        public static void SetResult (this IResult result, IResult res) {
            if (res != null) {
                result.Code = res.Code;
                result.Message = res.Message;
                result.Exception = res.Exception;
            }
        }
    }
}