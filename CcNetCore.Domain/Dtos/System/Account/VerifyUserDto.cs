using Newtonsoft.Json;

namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// 验证用户Dto
    /// </summary>
    public class VerifyUserDto {
        /// <summary>
        /// 用户名
        /// </summary>
        /// <value></value>
        [JsonRequired]
        public string UserName { get; set; }

        /// <summary>
        /// 登录密码哈希值
        /// </summary>
        /// <value></value>
        [JsonRequired]
        public string PasswordHash { get; set; }
    }
}