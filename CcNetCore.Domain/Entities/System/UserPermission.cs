using System.ComponentModel;
using CcNetCore.Common;
using CcNetCore.Utils.Converters;
using Dapper.Contrib.Extensions;

namespace CcNetCore.Domain.Entities {
    /// <summary>
    /// 角色权限表
    /// </summary>
    public class UserPermission : BaseEntity {
        /// <summary>
        /// 用户GUID
        /// </summary>
        [Column ("user_uid")]
        public string UserGuid { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        [Column ("perm_code")]
        public string PermCode { get; set; }

        /// <summary>
        /// 权限操作码
        /// </summary>
        [Column ("action_code")]
        public string ActionCode { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [Column ("perm_name")]
        public string PermName { get; set; }

        /// <summary>
        /// 权限类型(0:菜单,1:按钮/操作/功能等)
        /// </summary>
        [Column ("perm_type")]
        public PermType? PermType { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [Column ("menu_name")]
        public string MenuName { get; set; }

        /// <summary>
        /// 页面别名
        /// </summary>
        [Column ("menu_alias")]
        public string MenuAlias { get; set; }

        /// <summary>
        /// 菜单GUID
        /// </summary>
        [Column ("menu_uid")]
        public string MenuGuid { get; set; }

        /// <summary>
        /// 是否为默认路由
        /// </summary>
        [TypeConverter (typeof (NullableBoolTypeConverter))]
        [Column ("is_default")]
        public bool? IsDefaultRouter { get; set; }
    }
}