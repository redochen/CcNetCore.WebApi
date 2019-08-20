using System.ComponentModel.DataAnnotations;
using CcNetCore.Utils.Attributes;

using Schema = System.ComponentModel.DataAnnotations.Schema;

namespace CcNetCore.Domain.Entities {
    /// <summary>
    /// 角色权限表
    /// </summary>
    [Schema.Table ("role_permission")]
    public class RolePermission : BaseEntity {
        /// <summary>
        /// 角色编码
        /// </summary>
        [Required]
        [Column ("role_code", VarLength = 20)]
        public string RoleCode { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        [Column ("perm_code", VarLength = 20)]
        public string PermCode { get; set; }
    }
}