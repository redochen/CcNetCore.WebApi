namespace CcNetCore.Application.Interfaces {
    /// <summary>
    /// 分页查询接口
    /// </summary>
    public interface IPage {
        /// <summary>
        /// 每页显示记录数（小于或等于0表示不分页显示）
        /// </summary>
        /// <value></value>
        int PageSize { get; set; }

        /// <summary>
        /// 页码，从1开始
        /// </summary>
        /// <value></value>
        int PageNo { get; set; }
    }
}