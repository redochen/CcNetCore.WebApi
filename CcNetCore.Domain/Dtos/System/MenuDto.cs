namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// 菜单Dto
    /// </summary>
    public class MenuDto : BaseDto {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 页面别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 菜单图标(可选)
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 父级GUID
        /// /// </summary>
        public string ParentUid { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 菜单层级深度
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// 前端组件(.vue)
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// 页面关闭前的回调函数
        /// </summary>
        public string BeforeCloseFun { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// 是否为默认路由
        /// </summary>
        public bool? IsDefault { get; set; }

        /// <summary>
        /// 在菜单中隐藏
        /// </summary>
        public bool? HideInMenu { get; set; }

        /// <summary>
        /// 不缓存页面
        /// </summary>
        public bool? NotCache { get; set; }
    }
}