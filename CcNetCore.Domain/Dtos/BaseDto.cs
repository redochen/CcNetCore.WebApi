using System;
using CcNetCore.Common;

namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// Dto基类
    /// </summary>
    public class BaseDto {
        /// <summary>
        /// 惟一标识
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Status? Status { get; set; }

        /// <summary>
        /// 删除标识
        /// </summary>
        public int? IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public int CreateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        public int UpdateUser { get; set; }
    }
}