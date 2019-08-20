using System.Collections.Generic;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 分页结果类
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class PageQueryResult<TData> : BaseResult {
        /// <summary>
        /// 总记录数
        /// </summary>
        /// <value></value>
        public int TotalCount { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <value></value>
        public List<TData> Items { get; set; }
    }
}