using System;
using System.ComponentModel;
using System.Globalization;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.Utils.Converters {
    /// <summary>
    /// 布尔值转换器类
    /// </summary>
    public class NullableBoolTypeConverter : CustomTypeConverter {
        public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof (int) ||
                destinationType == typeof (Nullable<int>) ||
                destinationType == typeof (bool) ||
                destinationType == typeof (Nullable<bool>)) {
                return true;
            }

            return base.CanConvertTo (context, destinationType);
        }

        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (value != null && !(value is Nullable<bool>)) {
                throw WrongTypeException;
            }

            var flag = value as Nullable<bool>;

            //转换成字符串
            if (typeof (string) == destinationType) {
                return flag.HasValue? flag.ToString () : string.Empty;
            }

            if (typeof (int) == destinationType) {
                return (flag ?? false) ? 1 : 0;
            }

            if (typeof (Nullable<int>) == destinationType) {
                return flag.HasValue ? (int?) (flag.Value ? 1 : 0) : null;
            }

            if (typeof (bool) == destinationType) {
                return flag ?? false;
            }

            if (typeof (Nullable<bool>) == destinationType) {
                return flag;
            }

            return base.ConvertTo (context, culture, value, destinationType);
        }

        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof (string) ||
                sourceType == typeof (int) ||
                sourceType == typeof (Nullable<int>) ||
                sourceType == typeof (bool) ||
                sourceType == typeof (Nullable<bool>)) {
                return true;
            }

            return base.CanConvertFrom (context, sourceType);
        }

        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (null == value) {
                return null;
            }

            //从字符串解析
            if (value is string str) {
                if (!str.IsValid ()) {
                    return null;
                }

                if ("1" == str || str.EqualsEx ("true", ignoreCase : true)) {
                    return true;
                }

                if ("0" == str || str.EqualsEx ("false", ignoreCase : true)) {
                    return false;
                }

                return null;
            }

            if (value is int n) {
                return (n == 0) ? false : true;
            }

            if (value is bool b) {
                return b;
            }

            return base.ConvertFrom (context, culture, value);
        }
    }
}