using System;

namespace CcNetCore.Utils.Attributes {
    /// <summary>
    /// 列名属性
    /// </summary>
    [AttributeUsage (AttributeTargets.Property)]
    public class ColumnAttribute : System.ComponentModel.DataAnnotations.Schema.ColumnAttribute {
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

    /// <summary>
    /// Specifies that this field is a explicitly set primary key in the database
    /// </summary>
    [AttributeUsage (AttributeTargets.Property)]
    public class ExplicitKeyAttribute : Attribute { }

    /// <summary>
    /// 自动增长属性
    /// </summary>
    [AttributeUsage (AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute { }

    /// <summary>
    /// 指定数据库应忽略的字段属性
    /// </summary>
    [AttributeUsage (AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute { }

    /// <summary>
    /// Specifies whether a field is writable in the database.
    /// </summary>
    [AttributeUsage (AttributeTargets.Property)]
    public class WriteAttribute : Attribute {
        /// <summary>
        /// Specifies whether a field is writable in the database.
        /// </summary>
        /// <param name="write">Whether a field is writable in the database.</param>
        public WriteAttribute (bool write) {
            Write = write;
        }

        /// <summary>
        /// Whether a field is writable in the database.
        /// </summary>
        public bool Write { get; }
    }
}