using Newtonsoft.Json;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 保存用户角色模型
    /// </summary>
    public class SaveUserRoleModel {
        /// <summary>
        /// 用户GUID
        /// </summary>
        [JsonRequired]
        public string UserGuid { get; set; }

        /// <summary>
        /// 角色编码集合
        /// </summary>
        [JsonRequired]
        public string[] RoleCodes { get; set; }
    }
}