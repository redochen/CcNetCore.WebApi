using Dapper.Contrib.Extensions;
using Schema = System.ComponentModel.DataAnnotations.Schema;

namespace CcNetCore.Domain.Entities {
    /// <summary>
    /// 用户角色表
    /// </summary>
    [Schema.Table ("sys_user_roles")]
    public class UserRole : BaseEntity {
        /// <summary>
        /// 用户GUID
        /// </summary>
        [Column ("user_uid", VarLength = 50)]
        public string UserGuid { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        [Column ("role_code", VarLength = 20)]
        public string RoleCode { get; set; }
    }
}