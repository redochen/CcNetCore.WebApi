using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CcNetCore.Utils.Converters;
using Dapper.Contrib.Extensions;

using Schema = System.ComponentModel.DataAnnotations.Schema;

namespace CcNetCore.Domain.Entities {
    /// <summary>
    /// 菜单表
    /// </summary>
    [Schema.Table ("sys_menus")]
    public class Menu : BaseEntity {
        /// <summary>
        /// 菜单名称
        /// </summary>
        [Required, Column ("menu_name", VarLength = 50, Unicode = true)]
        public string Name { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [Column ("menu_url", VarLength = 500, Unicode = true)]
        public string Url { get; set; }

        /// <summary>
        /// 页面别名
        /// </summary>
        [Column ("menu_alias", VarLength = 100, Unicode = true)]
        public string Alias { get; set; }

        /// <summary>
        /// 菜单图标(可选)
        /// </summary>
        [Column ("menu_icon", VarLength = 200, Unicode = true)]
        public string Icon { get; set; }

        /// <summary>
        /// 父级GUID
        /// </summary>
        [Column ("parent_uid", VarLength = 50)]
        public string ParentUid { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [Column ("menu_desc", VarLength = 500, Unicode = true)]
        public string Description { get; set; }

        /// <summary>
        /// 前端组件(.vue)
        /// </summary>
        [Column ("component", VarLength = 255, Unicode = true)]
        public string Component { get; set; }

        /// <summary>
        /// 页面关闭前的回调函数
        /// </summary>
        [Column ("before_close_func", VarLength = 255, Unicode = true)]
        public string BeforeCloseFun { get; set; }

        /// <summary>
        /// 菜单层级深度
        /// </summary>
        [Column ("menu_level")]
        public int? Level { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Column ("menu_sort")]
        public int? Sort { get; set; }

        /// <summary>
        /// 是否为默认路由
        /// </summary>
        [TypeConverter (typeof (NullableBoolTypeConverter))]
        [Column ("is_default")]
        public bool? IsDefault { get; set; }

        /// <summary>
        /// 在菜单中隐藏
        /// </summary>
        [TypeConverter (typeof (NullableBoolTypeConverter))]
        [Column ("hide_in_menu")]
        public bool? HideInMenu { get; set; }

        /// <summary>
        /// 不缓存页面
        /// </summary>
        [TypeConverter (typeof (NullableBoolTypeConverter))]
        [Column ("no_cache")]
        public bool? NotCache { get; set; }
    }
}