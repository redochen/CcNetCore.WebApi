using System;

namespace Dapper.Contrib.Extensions {
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