using Newtonsoft.Json;

namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// 保存用户角色Dto
    /// </summary>
    public class SaveUserRoleDto {
        /// <summary>
        /// 用户GUID集合
        /// </summary>
        public string[] UserGuids { get; set; }

        /// <summary>
        /// 角色编码集合
        /// </summary>
        public string[] RoleCodes { get; set; }
    }
}