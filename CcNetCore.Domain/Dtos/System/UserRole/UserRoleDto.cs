namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// 用户角色Dto
    /// </summary>
    public class UserRoleDto : SysDto {
        /// <summary>
        /// 用户GUID
        /// </summary>
        public string UserGuid { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleCode { get; set; }
    }
}