using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CcNetCore.Utils.Attributes;
using CcNetCore.Utils.Converters;

using Schema = System.ComponentModel.DataAnnotations.Schema;

namespace CcNetCore.Domain.Entities {
    /// <summary>
    /// 角色表
    /// </summary>
    [Schema.Table ("roles")]
    public class Role : BaseEntity {
        /// <summary>
        /// 角色编码
        /// </summary>
        [Required]
        [Key]
        [Column ("role_code", VarLength = 20)]
        public string Code { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [Column ("role_name", VarLength = 50, Unicode = true)]
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [Column ("role_desc", VarLength = 500, Unicode = true)]
        public string Description { get; set; }

        /// <summary>
        /// 是否是超级管理员(超级管理员拥有系统的所有权限)
        /// </summary>
        [TypeConverter (typeof (NullableBoolTypeConverter))]
        [Column ("is_super_admin")]
        public bool? IsSuperAdmin { get; set; }

        /// <summary>
        /// 是否是系统内置角色(系统内置角色不允许删除,修改操作)
        /// </summary>
        [TypeConverter (typeof (NullableBoolTypeConverter))]
        [Column ("is_builtin")]
        public bool? IsBuiltin { get; set; }
    }
}