using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.Utils {
    /// <summary>
    /// 自定义格式化类型
    /// </summary>
    public enum CustomFormatType {
        None = 0, Custom, DateTime, Long, Decimal
    }

    /// <summary>
    /// 自定义格式化
    /// </summary>
    public class CustomFormat : IFormatProvider, ICustomFormatter {
        #region 常用格式定义
        /// <summary>
        /// 默认的日期
        /// </summary>
        public const string FORMAT_DATE = "yyyy-MM-dd";

        /// <summary>
        /// 默认的时间格式
        /// </summary>
        public const string FORMAT_TIME = "HH:mm:ss";

        /// <summary>
        /// 默认的日期-时间格式
        /// </summary>
        public const string FORMAT_DATE_TIME = FORMAT_DATE + " " + FORMAT_TIME;
        #endregion

        public static CustomFormat GetFormat (CustomFormatType type, string formatString) {
            switch (type) {
                case CustomFormatType.DateTime:
                    return new DateTimeFormat (formatString);
                case CustomFormatType.Long:
                    return new LongFormat (formatString);
                case CustomFormatType.Decimal:
                    return new DecimalFormat (formatString);
                case CustomFormatType.Custom:
                default:
                    return new CustomFormat (formatString);
            }
        }

        /// <summary>
        /// 格式字符串
        /// </summary>
        protected string FormatString = null;

        /// <summary>
        /// 错误信息
        /// </summary>
        public List<string> ErrorInfos { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatString"></param>
        public CustomFormat (string formatString) {
            FormatString = formatString;
            ErrorInfos = new List<string> ();
        }

        #region IFormatProvider接口
        public object GetFormat (Type formatType) {
            if (typeof (ICustomFormatter) == formatType) {
                return this;
            }

            return null;
        }
        #endregion

        #region ICustomFormatter接口
        public string Format (string format, object arg, IFormatProvider formatProvider) => Format (arg, true);
        #endregion

        /// <summary>
        /// 格式化输出
        /// </summary>
        /// <param name="value"></param>
        /// <param name="clearErrorInfo">是否清空错误信息</param>
        /// <returns></returns>
        public virtual string Format (object value, bool clearErrorInfo = true) {
            if (clearErrorInfo) //清空错误信息
            {
                ErrorInfos.Clear ();
            }

            return InnerFormat (value?.ToString ());
        }

        /// <summary>
        /// 校验格式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Validating (object value) {
            try {
                var formatValue = Format (null, value, null);
                return formatValue.Equals (value.ToSpecObject<string> ());
            } catch {
                return false;
            }
        }

        /// <summary>
        /// 格式化字符串（只做长度限制格式化）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string InnerFormat (string value) {
            if (!value.IsValid (false)) {
                return value;
            }

            TravesalFormatString ((fmt) => {
                if (fmt.IndexOf (FormatMinLength) >= 0) {
                    var minLength = fmt.Replace (FormatMinLength, string.Empty).ToInt32 ();
                    if (value.Length < minLength) {
                        ErrorInfos.Add ($"不足最小长度 {minLength}");

                        value = value.PadRight (minLength, ChrZero);
                        return true; //break;
                    }

                    return false; //continue;
                }

                if (fmt.IndexOf (FormatMaxLength) >= 0) {
                    var maxLength = fmt.Replace (FormatMaxLength, string.Empty).ToInt32 ();
                    if (value.Length > maxLength) {
                        ErrorInfos.Add ($"超过最大长度 {maxLength}");

                        value = value.Substring (0, maxLength);
                        return true; //break;
                    }

                    return false; //continue;
                }

                return false; //continue;
            });

            return value;
        }

        /// <summary>
        /// 获取特定的格式字符串
        /// </summary>
        /// <param name="formatKey">格式键</param>
        /// <returns></returns>
        protected string GetFormatString (string formatKey) {
            if (!formatKey.IsValid ()) {
                return string.Empty;
            }

            string fmtString = null;

            TravesalFormatString ((fmt) => {
                if (fmt.IndexOf (formatKey) < 0) {
                    return false; //continue;
                }

                fmtString = fmt.Replace (formatKey, string.Empty);
                return true; //break;
            });

            return fmtString;
        }

        /// <summary>
        /// 遍历所有格式字符串
        /// </summary>
        /// <param name="funcCheckFormat"></param>
        protected void TravesalFormatString (Func<string /*fmt*/ , bool /*break*/> funcCheckFormat) {
            var formats = FormatString.Split (new char[] { ChrSeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (formats.IsEmpty ()) {
                return;
            }

            foreach (var fmt in formats) {
                if (!fmt.IsValid ()) {
                    continue;
                }

                var shouldBreak = funcCheckFormat (fmt);
                if (shouldBreak) {
                    break;
                }
            }
        }

        #region 获取格式化实例
        /// <summary>
        /// 获取日期格式化器实例
        /// </summary>
        /// <param name="format">日期格式</param>
        /// <returns></returns>
        public static DateTimeFormat DateFormat (string format = FORMAT_DATE) => new DateTimeFormat (format);

        /// <summary>
        /// 获取时间格式化器实例
        /// </summary>
        /// <param name="format">时间格式</param>
        /// <returns></returns>
        public static DateTimeFormat TimeFormat (string format = FORMAT_TIME) => new DateTimeFormat (format);

        /// <summary>
        /// 获取日期时间格式化器实例
        /// </summary>
        /// <param name="format">日期时间格式</param>
        /// <returns></returns>
        public static DateTimeFormat DateTimeFormat (string format = FORMAT_DATE_TIME) => new DateTimeFormat (format);

        /// <summary>
        /// 获取长度限制格式化器实例
        /// </summary>
        /// <param name="minLength">最小长度</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns></returns>
        public static CustomFormat LengthFormat (uint? minLength = null, uint? maxLength = null) => new CustomFormat (GetLengthFormatString (minLength, maxLength));

        /// <summary>
        /// 获取整数取值限制格式器实例
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minLength">最小长度</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns></returns>
        public static LongFormat NumberFormat (long? minValue = null,
            long? maxValue = null, uint? minLength = null, uint? maxLength = null) {
            var sbFormat = new StringBuilder ();

            sbFormat.Append (GetLengthFormatString (minLength, maxLength), ChrSeparator);
            sbFormat.Append (GetValueFormatString (minValue, maxValue), ChrSeparator);

            return new LongFormat (sbFormat.ToString ());
        }

        /// <summary>
        /// 获取小数取值限制格式器实例
        /// </summary>
        /// <param name="decimalPlace">小数位精度</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minLength">最小长度</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns></returns>
        public static DecimalFormat DecimalFormat (uint? decimalPlace = null, decimal? minValue = null,
            decimal? maxValue = null, uint? minLength = null, uint? maxLength = null) {
            var sbFormat = new StringBuilder ();

            sbFormat.Append (GetDecimalFormatString (decimalPlace ?? 0), ChrSeparator);
            sbFormat.Append (GetLengthFormatString (minLength, maxLength), ChrSeparator);
            sbFormat.Append (GetValueFormatString (minValue, maxValue), ChrSeparator);

            return new DecimalFormat (sbFormat.ToString ());
        }
        #endregion

        #region 获取格式字符串
        /// <summary>
        /// 获取长度限制格式字符串
        /// </summary>
        /// <param name="minLength">最小长度</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns></returns>
        private static string GetLengthFormatString (uint? minLength = null, uint? maxLength = null) {
            if (minLength.HasValue && maxLength.HasValue &&
                maxLength.Value < minLength.Value) {
                throw new Exception ("最大长度不能小于最小长度");
            }

            var sbFormat = new StringBuilder ();

            if (minLength.HasValue) {
                sbFormat.Append (FormatMinLength, ChrSeparator).Append (minLength.Value);
            }

            if (maxLength.HasValue) {
                sbFormat.Append (FormatMaxLength, ChrSeparator).Append (maxLength.Value);
            }

            return sbFormat.ToString ();
        }

        /// <summary>
        /// 获取整数取值限制格式字符串
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns></returns>
        private static string GetValueFormatString (long? minValue = null, long? maxValue = null) {
            if (minValue.HasValue && maxValue.HasValue &&
                maxValue.Value < minValue.Value) {
                throw new Exception ("最大取值不能小于最小取值");
            }

            var sbFormat = new StringBuilder ();

            if (minValue.HasValue) {
                sbFormat.Append (FormatMinValue, ChrSeparator).Append (minValue.Value);
            }

            if (maxValue.HasValue) {
                sbFormat.Append (FormatMaxValue, ChrSeparator).Append (maxValue.Value);
            }

            return sbFormat.ToString ();
        }

        /// <summary>
        /// 获取小数取值限制格式字符串
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns></returns>
        private static string GetValueFormatString (decimal? minValue = null, decimal? maxValue = null) {
            if (minValue.HasValue && maxValue.HasValue &&
                maxValue.Value < minValue.Value) {
                throw new Exception ("最大取值不能小于最小取值");
            }

            var sbFormat = new StringBuilder ();

            if (minValue.HasValue) {
                sbFormat.Append (FormatMinValue, ChrSeparator).Append (minValue.Value);
            }

            if (maxValue.HasValue) {
                sbFormat.Append (FormatMaxValue, ChrSeparator).Append (maxValue.Value);
            }

            return sbFormat.ToString ();
        }

        /// <summary>
        /// 获取小数位限制格式字符串
        /// </summary>
        /// <param name="decimalPlace">小数位精度</param>
        /// <returns></returns>
        private static string GetDecimalFormatString (uint decimalPlace) {
            if (decimalPlace <= 0) {
                return string.Empty;
            }

            return $"{FormatFormat}{PrefixFloat.PadRight((int)decimalPlace + PrefixFloat.Length, ChrSharp)}";
        }
        #endregion

        public const char ChrDot = '.';
        public const char ChrSharp = '#';
        public const char ChrSeparator = (char) 1;
        protected const string PrefixFloat = "0.";
        protected const char ChrZero = '0';
        protected const string FormatFormat = "__Format__=";
        protected const string FormatMinLength = "__MinLen__=";
        protected const string FormatMaxLength = "__MaxLen__=";
        protected const string FormatMinValue = "__MinValue__=";
        protected const string FormatMaxValue = "__MaxValue__=";
    }

    /// <summary>
    /// 日期时间格式化
    /// </summary>
    public class DateTimeFormat : CustomFormat {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatString"></param>
        public DateTimeFormat (string formatString) : base (formatString) { }

        /// <summary>
        /// 格式化输出
        /// </summary>
        /// <param name="value"></param>
        /// <param name="clearErrorInfo">是否清空错误信息</param>
        /// <returns></returns>
        public override string Format (object value, bool clearErrorInfo = true) {
            try {
                if (clearErrorInfo) //清空错误信息
                {
                    ErrorInfos.Clear ();
                }

                var str = value.ToSpecObject<string> ();
                if (!str.IsValid (false)) {
                    return string.Empty;
                }

                var orgStr = str;
                if (DateTime.TryParse (str, out DateTime dt)) {
                    str = dt.ToString (FormatString);
                } else {
                    str = string.Empty;
                }

                if (!orgStr.Equals (str)) {
                    ErrorInfos.Add ($"日期时间格式应为 {GetReadableText(FormatString)}");
                }

                return str;
            } catch (Exception ex) {
                return ex.Message;
            }
        }

        #region 格式可读性转换
        private string GetReadableText (string format) {
            if (!format.IsValid ()) {
                return string.Empty;
            }

            foreach (var kvp in FormatSamples) {
                if (format.Contains (kvp.Key)) {
                    format = format.Replace (kvp.Key, kvp.Value);
                }
            }

            return format;
        }

        private static Dictionary<string, string> FormatSamples = new Dictionary<string, string> {
            /*年*/
            ["yyyy"] = "2000",
            ["yy"] = "20",
            /*月*/
            ["MM"] = "01",
            ["M"] = "1",
            /*日*/
            ["dd"] = "01",
            ["d"] = "1",
            /*时*/
            ["HH"] = "01",
            ["H"] = "1",
            /*分*/
            ["mm"] = "01",
            ["m"] = "1",
            /*秒*/
            ["ss"] = "01",
            ["s"] = "1",
        };
        #endregion
    }

    /// <summary>
    /// 数字格式化
    /// </summary>
    public abstract class NumberFormat : CustomFormat {
        /// <summary>
        /// 数字类型
        /// </summary>
        protected abstract Type NumberType { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatString"></param>
        public NumberFormat (string formatString) : base (formatString) { }

        /// <summary>
        /// 格式化输出
        /// </summary>
        /// <param name="value"></param>
        /// <param name="clearErrorInfo">是否清空错误信息</param>
        /// <returns></returns>
        public override string Format (object value, bool clearErrorInfo = true) {
            try {
                if (clearErrorInfo) //清空错误信息
                {
                    ErrorInfos.Clear ();
                }

                var str = value?.ToString ();
                if (!str.IsValid (false)) {
                    return string.Empty;
                }

                var number = Convert.ChangeType (str, NumberType);
                var fmt = GetFormatString (FormatFormat);
                if (!fmt.IsValid ()) {
                    return base.Format (value, false);
                }

                var data = number.Invoke ("ToString", new [] { fmt }, new Type[] { typeof (string) });
                if (!number.Equals (data)) {
                    ErrorInfos.Add ($"最多只能输入 {GetReadableText(fmt)} 位小数");
                }

                return base.Format (data, false);
            } catch (FormatException fe) {
                ErrorInfos.Add ("格式不正确");
                return fe.Message;
            } catch (Exception ex) {
                ErrorInfos.Add (ex.Message);
                return ex.Message;
            }
        }

        #region 格式可读性转换
        private string GetReadableText (string format) {
            if (!format.IsValid ()) {
                return string.Empty;
            }

            return format.Count (x => x == ChrSharp).ToString ();
        }
        #endregion
    }

    /// <summary>
    /// 整数格式化
    /// </summary>
    public class LongFormat : NumberFormat {
        /// <summary>
        /// 数字类型
        /// </summary>
        protected override Type NumberType => typeof (long);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatString"></param>
        public LongFormat (string formatString) : base (formatString) { }

        /// <summary>
        /// 格式化输出
        /// </summary>
        /// <param name="value"></param>
        /// <param name="clearErrorInfo">是否清空错误信息</param>
        /// <returns></returns>
        public override string Format (object value, bool clearErrorInfo = true) {
            try {
                if (clearErrorInfo) //清空错误信息
                {
                    ErrorInfos.Clear ();
                }

                var str = value?.ToString ();
                if (!str.IsValid (false)) {
                    return string.Empty;
                }

                var number = Convert.ToInt64 (str);
                var orgNumber = number;

                var minValue = GetFormatString (FormatMinValue)?.ToLong ();
                if (minValue != null) {
                    number = Math.Max (number, minValue.Value);
                }

                var maxValue = GetFormatString (FormatMaxValue)?.ToLong ();
                if (maxValue != null) {
                    number = Math.Min (number, maxValue.Value);
                }

                if (number != orgNumber) {
                    if (minValue != null && maxValue != null) {
                        ErrorInfos.Add ($"只能输入 {minValue.Value} ~ {maxValue.Value} 之间的值");
                    } else if (minValue != null) {
                        ErrorInfos.Add ($"只能输入大于或等于 {minValue.Value} 的值");
                    } else if (maxValue != null) {
                        ErrorInfos.Add ($"只能输入小于或等于 {maxValue.Value} 的值");
                    }

                    return number.ToString ();
                }

                return base.Format (value, false);
            } catch (FormatException fe) {
                ErrorInfos.Add ("格式不正确");
                return fe.Message;
            } catch (Exception ex) {
                ErrorInfos.Add (ex.Message);
                return ex.Message;
            }
        }
    }

    /// <summary>
    /// 小数格式化
    /// </summary>
    public class DecimalFormat : NumberFormat {
        /// <summary>
        /// 数字类型
        /// </summary>
        protected override Type NumberType => typeof (decimal);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatString"></param>
        public DecimalFormat (string formatString) : base (formatString) { }

        /// <summary>
        /// 格式化输出
        /// </summary>
        /// <param name="value"></param>
        /// <param name="clearErrorInfo">是否清空错误信息</param>
        /// <returns></returns>
        public override string Format (object value, bool clearErrorInfo = true) {
            try {
                if (clearErrorInfo) //清空错误信息
                {
                    ErrorInfos.Clear ();
                }

                var str = value?.ToString ();
                if (!str.IsValid (false)) {
                    return string.Empty;
                }

                var number = Convert.ToDecimal (str);
                var orgNumber = number;

                var minValue = GetFormatString (FormatMinValue)?.ToDecimal ();
                if (minValue != null) {
                    number = Math.Max (number, minValue.Value);
                }

                var maxValue = GetFormatString (FormatMaxValue)?.ToDecimal ();
                if (maxValue != null) {
                    number = Math.Min (number, maxValue.Value);
                }

                if (number != orgNumber) {
                    if (minValue != null && maxValue != null) {
                        ErrorInfos.Add ($"只能输入 {minValue.Value} ~ {maxValue.Value} 之间的值");
                    } else if (minValue != null) {
                        ErrorInfos.Add ($"只能输入大于或等于 {minValue.Value} 的值");
                    } else if (maxValue != null) {
                        ErrorInfos.Add ($"只能输入小于或等于 {maxValue.Value} 的值");
                    }

                    return number.ToString ();
                }

                return base.Format (value, false);
            } catch (FormatException fe) {
                ErrorInfos.Add ("格式不正确");
                return fe.Message;
            } catch (Exception ex) {
                ErrorInfos.Add (ex.Message);
                return ex.Message;
            }
        }
    }
}