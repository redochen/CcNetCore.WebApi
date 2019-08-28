using CcNetCore.Common;

namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// 用户Dto
    /// </summary>
    public class UserDto : BaseDto {
        /// <summary>
        /// 登录名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 显示昵名
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 登录密码哈希值
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// 显示头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType? UserType { get; set; }

        /// <summary>
        /// 用户描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否已锁定
        /// </summary>
        /// <value></value>
        public bool? IsLocked { get; set; }
    }
}