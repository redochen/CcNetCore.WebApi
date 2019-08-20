using Newtonsoft.Json;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 保存角色权限模型
    /// </summary>
    public class SaveRolePermModel {
        /// <summary>
        /// 角色编码
        /// </summary>
        [JsonRequired]
        public string RoleCode { get; set; }

        /// <summary>
        /// 权限编码集合
        /// </summary>
        [JsonRequired]
        public string[] PermCodes { get; set; }
    }
}