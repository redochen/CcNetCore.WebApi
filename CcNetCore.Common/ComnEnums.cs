namespace CcNetCore.Common {
    /// <summary>
    /// 状态
    /// </summary>
    public enum Status {
        /// <summary>
        /// 未指定
        /// </summary>
        All = -1,

        /// <summary>
        /// 已禁用
        /// </summary>
        Forbidden = 0,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 1
    }

    /// <summary>
    /// 审核状态
    /// </summary>
    public enum AuditStatus {
        /// <summary>
        /// 未指定
        /// </summary>
        All = -1,

        /// <summary>
        /// 未审核
        /// </summary>
        UnAudited = 0,

        /// <summary>
        /// 已审核
        /// </summary>
        Audited = 1,
    }

    /// <summary>
    /// 用户类型
    /// </summary>
    public enum UserType {
        /// <summary>
        /// 一般用户
        /// </summary>
        General = 0,

        /// <summary>
        /// 管理员
        /// </summary>
        Admin = 1,

        /// <summary>
        /// 超级管理员
        /// </summary>
        SuperAdmin = 2,
    }

    /// <summary>
    /// 权限类型
    /// </summary>
    public enum PermType {
        /// <summary>
        /// 菜单
        /// </summary>
        Menu = 0,

        /// <summary>
        /// 按钮/操作/功能
        /// </summary>
        Action = 1
    }
}