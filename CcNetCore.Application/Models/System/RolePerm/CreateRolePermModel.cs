using Newtonsoft.Json;
using CcNetCore.Application.Interfaces;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 创建角色权限模型
    /// </summary>
    public class CreateRolePermModel : ICreateModel {
        /// <summary>
        /// 角色编码
        /// </summary>
        [JsonRequired]
        public string RoleCode { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        [JsonRequired]
        public string PermCode { get; set; }
    }
}