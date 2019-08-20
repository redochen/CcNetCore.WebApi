namespace CcNetCore.Application.Models {
    /// <summary>
    /// 用户角色模型
    /// </summary>
    public class UserRoleModel : BaseModel {
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