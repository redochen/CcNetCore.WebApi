using CcNetCore.Application.Interfaces;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 分页查询模块
    /// </summary>
    public class Query<TQuery> : IPage {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Query () {
            PageSize = 0;
            PageNo = 0;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="page"></param>
        public Query (IPage page) {
            PageSize = page?.PageSize ?? 0;
            PageNo = page?.PageNo ?? 1;
        }

        /// <summary>
        /// 每页显示记录数（小于或等于0表示不分页显示）
        /// </summary>
        /// <value></value>
        public int PageSize { get; set; }

        /// <summary>
        /// 页码，从1开始
        /// </summary>
        /// <value></value>
        public int PageNo { get; set; }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <value></value>
        public TQuery Condition { get; set; }
    }
}