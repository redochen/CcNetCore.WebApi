namespace CcNetCore.Application.Models {
    /// <summary>
    /// 分页查询模块
    /// </summary>
    public class PageQueryModel<TQuery> : BaseModel {
        public PageQueryModel () {
            PageSize = 0;
            PageIndex = 0;
        }

        public static PageQueryModel<TQuery> Parse<T> (PageQueryModel<T> query) {
            var model = new PageQueryModel<TQuery> {
                PageSize = query?.PageSize??0,
                PageIndex = query?.PageIndex??0,
            };

            return model;
        }

        /// <summary>
        /// 每页显示记录数（小于或等于0表示不分页显示）
        /// </summary>
        /// <value></value>
        public int PageSize { get; set; }

        /// <summary>
        /// 页码，从0开始
        /// </summary>
        /// <value></value>
        public int PageIndex { get; set; }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <value></value>
        public TQuery Condition { get; set; }
    }
}