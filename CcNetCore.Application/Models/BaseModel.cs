using CcNetCore.Common;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 模型基类
    /// </summary>
    public class BaseModel {
        /// <summary>
        /// 惟一标识
        /// </summary>
        /// <value></value>
        public string Uid { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Status? Status { get; set; }
    }
}