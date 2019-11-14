using CcNetCore.Application.Interfaces;
using CcNetCore.Common;
using Newtonsoft.Json;

namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// 创建用户Dto
    /// </summary>
    public class CreateUserDto : ICreateDto {
        /// <summary>
        /// 登录名
        /// </summary>
        [JsonRequired]
        public string UserName { get; set; }

        /// <summary>
        /// 显示昵名
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 登录密码哈希值
        /// </summary>
        /// <value></value>
        [JsonRequired]
        public string PasswordHash { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType? UserType { get; set; }

        /// <summary>
        /// 用户描述
        /// </summary>
        public string Description { get; set; }
    }
}