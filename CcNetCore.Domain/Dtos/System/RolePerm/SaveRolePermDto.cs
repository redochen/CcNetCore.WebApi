using Newtonsoft.Json;

namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// 保存角色权限Dto
    /// </summary>
    public class SaveRolePermDto {
        /// <summary>
        /// 角色编码集合
        /// </summary>
        public string[] RoleCodes { get; set; }

        /// <summary>
        /// 权限编码集合
        /// </summary>
        public string[] PermCodes { get; set; }
    }
}