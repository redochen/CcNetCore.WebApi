using System.ComponentModel;

namespace CcNetCore.Common {
    /// <summary>
    /// 错误代码枚举
    /// </summary>
    public enum ErrorCode {
        [Description ("成功")]
        Success = 0,

        [Description ("失败")]
        Failed = 1,

        [Description ("不支持")]
        UnSupported,

        [Description ("未授权")]
        UnAuthorised,

        [Description ("无效的令牌")]
        TokenNotFound,

        [Description ("令牌为空")]
        InvalidToken,

        [Description ("令牌已过期")]
        ExpiredToken,

        [Description ("无效的参数")]
        InvalidParam,
    }
}