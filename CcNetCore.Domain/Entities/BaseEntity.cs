using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace CcNetCore.Domain.Entities {
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class BaseEntity {
        /// <summary>
        /// 自增长标识
        /// </summary>
        /// <value></value>
        [AutoIncrement]
        [ExplicitKey]
        [Column ("id", Order = 0)]
        public int Id { get; set; }

        /// <summary>
        /// 惟一标识
        /// </summary>
        [Key]
        [Column ("uid", VarLength = 50, Order = 1)]
        public string Uid { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Column ("status", Order = 101)]
        public int? Status { get; set; }

        /// <summary>
        /// 删除标识
        /// </summary>
        [Column ("del_flag", Order = 102)]
        public int? IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column ("create_time", Order = 103)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        [Column ("create_user", Order = 104)]
        public int CreateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column ("update_time", Order = 105)]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        [Column ("update_user", Order = 106)]
        public int UpdateUser { get; set; }

        /// <summary>
        /// 过滤条件：最小创建时间（含）
        /// </summary>
        /// <value></value>
        [Ignore]
        public string MinCreateTime { get; set; }

        /// <summary>
        /// 过滤条件：最大创建时间（含）
        /// </summary>
        /// <value></value>
        [Ignore]
        public string MaxCreateTime { get; set; }
    }
}