using CcNetCore.Common;
using Newtonsoft.Json;

namespace CcNetCore.Domain.Dtos {
    /// <summary>
    /// Dto基类
    /// </summary>
    public class BaseDto {
        /// <summary>
        /// 惟一标识
        /// /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public int? CreateUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        public int? UpdateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime { get; set; }

        /// <summary>
        /// 过滤条件：最小创建时间（含）
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public string MinCreateTime { get; set; }

        /// <summary>
        /// 过滤条件：最大创建时间（含）
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public string MaxCreateTime { get; set; }
    }

    /// <summary>
    /// 系统Dto基类
    /// </summary>
    public class SysDto : BaseDto {
        /// <summary>
        /// 状态
        /// </summary>
        public Status? Status { get; set; }
    }

    /// <summary>
    /// 业务Dto基类
    /// </summary>
    public class BizDto : BaseDto {
        /// <summary>
        /// 审核状态
        /// </summary>
        public AuditStatus? Status { get; set; }
    }
}