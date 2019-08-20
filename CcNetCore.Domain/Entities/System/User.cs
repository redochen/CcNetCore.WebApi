using System.ComponentModel.DataAnnotations;
using CcNetCore.Common;
using CcNetCore.Utils.Attributes;

using Schema = System.ComponentModel.DataAnnotations.Schema;

namespace CcNetCore.Domain.Entities {
    /// <summary>
    /// 用户表
    /// </summary>
    [Schema.Table ("users")]
    public class User : BaseEntity {
        /// <summary>
        /// 登录名
        /// </summary>
        [Required]
        [Column ("user_name", VarLength = 50, Unicode = true)]
        public string UserName { get; set; }

        /// <summary>
        /// 显示昵名
        /// </summary>
        [Column ("nick_name", VarLength = 50, Unicode = true)]
        public string NickName { get; set; }

        /// <summary>
        /// 登录密码哈希值
        /// </summary>
        [Column ("pwd_hash", VarLength = 100)]
        public string PasswordHash { get; set; }

        /// <summary>
        /// 显示头像
        /// </summary>
        [Column ("avatar", VarLength = 255, Unicode = true)]
        public string Avatar { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        [Column ("user_type")]
        public UserType? UserType { get; set; }

        /// <summary>
        /// 用户描述
        /// </summary>
        [Column ("user_desc", VarLength = 500, Unicode = true)]
        public string Description { get; set; }
    }
}