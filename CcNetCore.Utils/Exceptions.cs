using System;

namespace CcNetCore.Utils {
    /// <summary>
    /// 异常列表静态类
    /// </summary>
    public static class Exceptions {
        public static readonly FailureException Failure = new FailureException ();
        public static readonly NotImplementedException NotImplemented = new NotImplementedException ();
        public static readonly InvalidParamException InvalidParam = new InvalidParamException ();
        public static readonly NotFoundException NotFound = new NotFoundException ();
        public static readonly AlreadyExistsException AlreadyExists = new AlreadyExistsException ();
        public static readonly IdentityException Indentify = new IdentityException ();
        public static readonly UnauthorizedException Unauthorized = new UnauthorizedException ();
    }

    /// <summary>
    /// 操作失败
    /// </summary>
    public class FailureException : Exception {
        public FailureException () : base ("操作失败") { }
    }

    /// <summary>
    /// 未实现
    /// </summary>
    public class NotImplementedException : Exception {
        public NotImplementedException () : base ("未实现") { }
    }

    /// <summary>
    /// 无效的参数
    /// </summary>
    public class InvalidParamException : Exception {
        public InvalidParamException () : base ("无效的参数") { }
    }

    /// <summary>
    /// 目标未找到
    /// </summary>
    public class NotFoundException : Exception {
        public NotFoundException () : base ("目标未找到") { }
    }

    /// <summary>
    /// 目标已存在
    /// </summary>
    public class AlreadyExistsException : Exception {
        public AlreadyExistsException () : base ("目标已存在") { }
    }

    /// <summary>
    /// 密码错误
    /// </summary>
    public class IdentityException : Exception {
        public IdentityException () : base ("密码错误") { }
    }

    /// <summary>
    /// 操作未授权
    /// </summary>
    public class UnauthorizedException : Exception {
        public UnauthorizedException () : base ("操作未授权") { }
    }
}