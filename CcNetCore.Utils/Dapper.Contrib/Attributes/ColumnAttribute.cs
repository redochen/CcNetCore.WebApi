using System;

using Schema = System.ComponentModel.DataAnnotations.Schema;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// 列名属性
    /// </summary>
    [AttributeUsage (AttributeTargets.Property)]
    public class ColumnAttribute : Schema.ColumnAttribute {
        public ColumnAttribute (string columnName) : base (columnName) {
            Order = 100;
        }

        /// <summary>
        /// 最大长度（固定长度）
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 最大长度（不定长度）
        /// </summary>
        /// <value></value>
        public int VarLength { get; set; }

        /// <summary>
        /// 是否为Unicode字符
        /// </summary>
        /// <value></value>
        public bool Unicode { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }
    }
}