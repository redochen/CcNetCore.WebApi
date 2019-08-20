using Newtonsoft.Json;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 验证用户模型
    /// </summary>
    public class VerifyUserModel {
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