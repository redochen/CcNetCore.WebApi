using Newtonsoft.Json;

namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// 修改密码模块
    /// </summary>
    public class ChangePwdDto {
        /// <summary>
        /// 惟一标识
        /// </summary>
        /// <value></value>
        [JsonRequired]
        public string Uid { get; set; }

        /// <summary>
        /// 新密码哈希值
        /// </summary>
        /// <value></value>
        [JsonRequired]
        public string NewPasswordHash { get; set; }

        /// <summary>
        /// 新密码哈希值
        /// </summary>
        /// <value></value>
        [JsonRequired]
        public string OldPasswordHash { get; set; }
    }
}