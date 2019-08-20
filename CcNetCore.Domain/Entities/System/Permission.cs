using System.ComponentModel.DataAnnotations;
using CcNetCore.Common;
using CcNetCore.Utils.Attributes;

using Schema = System.ComponentModel.DataAnnotations.Schema;

namespace CcNetCore.Domain.Entities {
    /// <summary>
    /// 权限表
    /// </summary>
    [Schema.Table ("permission")]
    public class Permission : BaseEntity {
        /// <summary>
        /// 权限编码
        /// </summary>
        [Required]
        [Key]
        [Column ("perm_code", VarLength = 20)]
        public string Code { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [Required]
        [Column ("perm_name", VarLength = 50, Unicode = true)]
        public string Name { get; set; }

        /// <summary>
        /// 权限类型(0:菜单,1:按钮/操作/功能等)
        /// </summary>
        [Column ("perm_type")]
        public PermType? Type { get; set; }

        /// <summary>
        /// 菜单GUID
        /// </summary>
        [Column ("menu_uid", VarLength = 50)]
        public string MenuGuid { get; set; }

        /// <summary>
        /// 权限操作码
        /// </summary>
        [Required]
        [Column ("action_code", VarLength = 100)]
        public string ActionCode { get; set; }

        /// <summary>
        /// 图标(可选)
        /// </summary>
        [Column ("perm_icon", VarLength = 200, Unicode = true)]
        public string Icon { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [Column ("perm_desc", VarLength = 500, Unicode = true)]
        public string Description { get; set; }
    }
}