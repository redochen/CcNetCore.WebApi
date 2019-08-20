using Newtonsoft.Json;
using CcNetCore.Application.Interfaces;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 创建用户角色模型
    /// </summary>
    public class CreateUserRoleModel : ICreateModel {
        /// <summary>
        /// 用户GUID
        /// </summary>
        [JsonRequired]
        public string UserGuid { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        [JsonRequired]
        public string RoleCode { get; set; }
    }
}