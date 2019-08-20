namespace CcNetCore.Application.Models {
    /// <summary>
    /// 角色权限模型
    /// </summary>
    public class RolePermModel : BaseModel {
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