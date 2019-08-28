using System;
using System.ComponentModel;

namespace CcNetCore.Utils.Converters {
    /// <summary>
    /// 自定义类型转换器类
    /// </summary>
    public class CustomTypeConverter : TypeConverter {
        protected static Exception WrongTypeException = new Exception ("类型错误");
    }
}