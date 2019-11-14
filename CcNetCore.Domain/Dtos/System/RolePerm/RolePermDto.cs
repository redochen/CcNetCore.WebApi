namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// 角色权限Dto
    /// </summary>
    public class RolePermDto : SysDto {
        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        public string PermCode { get; set; }
    }
}