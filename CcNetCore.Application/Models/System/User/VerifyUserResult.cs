using Newtonsoft.Json;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 验证用户结果
    /// </summary>
    public class VerifyUserResult : BaseResult {
        /// <summary>
        /// 用户ID
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public int UserID { get; set; }
    }
}