using CcNetCore.Common;

namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// 用户权限Dto
    /// </summary>
    public class UserPermDto : SysDto {
        /// <summary>
        /// 用户GUID
        /// </summary>
        public string UserGuid { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        public string PermCode { get; set; }

        /// <summary>
        /// 权限操作码
        /// </summary>
        public string ActionCode { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string PermName { get; set; }

        /// <summary>
        /// 权限类型(0:菜单,1:按钮/操作/功能等)
        /// </summary>
        public PermType? PermType { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 页面别名
        /// </summary>
        public string MenuAlias { get; set; }

        /// <summary>
        /// 菜单GUID
        /// </summary>
        public string MenuGuid { get; set; }

        /// <summary>
        /// 是否为默认路由
        /// </summary>
        public bool? IsDefaultRouter { get; set; }
    }
}