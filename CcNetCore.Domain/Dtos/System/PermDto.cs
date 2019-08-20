using CcNetCore.Common;

namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// 权限Dto
    /// </summary>
    public class PermDto : BaseDto {
        /// <summary>
        /// 权限编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限类型(0:菜单,1:按钮/操作/功能等)
        /// </summary>
        public PermType? Type { get; set; }

        /// <summary>
        /// /// 菜单GUID
        /// </summary>
        public string MenuGuid { get; set; }

        /// <summary>
        /// 权限操作码
        /// </summary>
        public string ActionCode { get; set; }

        /// <summary>
        /// 图标(可选)
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }
    }
}