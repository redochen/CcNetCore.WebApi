#pragma warning disable CS0168

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace CcNetCore.Utils.Extensions {
    /// <summary>
    /// String扩展类
    /// </summary>
    public static class StringExtension {
        /// <summary>
        /// 是否有效（非null且非空）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="whiteSpaceAsEmpty">是否将空白字符视为空</param>
        /// <returns></returns>
        public static bool IsValid (this string str, bool whiteSpaceAsEmpty = true) {
            if (null == str) {
                return false;
            }

            if (whiteSpaceAsEmpty) {
                return !string.IsNullOrWhiteSpace (str);
            } else {
                return !string.IsNullOrEmpty (str);
            }
        }

        /// <summary>
        /// 判断两个字符串是否相同
        /// </summary>
        /// <param name="str"></param>
        /// <param name="other"></param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <param name="sameIfBothNotValid">两者都无效时返回相同</param>
        /// <returns></returns>
        public static bool EqualsEx (this string str, string other, bool ignoreCase = false, bool sameIfBothNotValid = true) {
            var thisValid = str.IsValid (true);
            var otherValid = other.IsValid (true);

            if (thisValid ^ otherValid) {
                return false;
            }

            if (!thisValid && !otherValid) {
                return sameIfBothNotValid;
            }

            if (ignoreCase) {
                return str.Equals (other, StringComparison.OrdinalIgnoreCase);
            }

            return str.Equals (other);
        }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encodingName">编码名称</param>
        /// <returns></returns>
        public static byte[] GetBytes (this string str, string encodingName = "UTF-8") {
            if (!str.IsValid ()) {
                return null;
            }

            return Encoding.GetEncoding (encodingName).GetBytes (str);
        }

        /// <summary>
        /// 获取非null的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="trimSapce">是否移除前后空格</param>
        /// <returns></returns>
        public static string GetValue (this string str, bool trimSapce = true) {
            if (null == str) {
                return string.Empty;
            }

            return trimSapce ? str.Trim () : str;
        }

        /// <summary>
        /// 获取非null的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="trimSpace">是否移除前后空格</param>
        /// <returns></returns>
        public static string GetValue (this string str, string defaultValue, bool trimSpace = true) {
            return str.IsValid (true) ? str.GetValue (trimSpace) : defaultValue.GetValue (trimSpace);
        }

        /// <summary>
        /// 获取大写值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="trim">是否移除前后空格</param>
        /// <returns></returns>
        public static string UpperCase (this string str, bool trimSapce = true) => str.GetValue (trimSapce).ToUpper ();

        /// <summary>
        /// 获取小写值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="trim">是否移除前后空格</param>
        /// <returns></returns>
        public static string LowerCase (this string str, bool trimSapce = true) => str.GetValue (trimSapce).ToLower ();

        /// <summary>
        /// 获取字符串集合中的第N个
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="index">索引号</param>
        /// <param name="trimSapce">却除前后空格</param>
        /// <returns></returns>
        public static string GetValue (this IEnumerable<string> strArray, int index, bool trimSapce) {
            if (strArray.IsEmpty ()) {
                return string.Empty;
            }

            //索引越界
            if (index < 0 || index >= strArray.Count ()) {
                return string.Empty;
            }

            return strArray.ElementAt (index).GetValue (trimSapce);
        }

        /// <summary>
        /// 获取第一个非空的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public static string GetFirstValid (this string str, params string[] others) {
            if (str.IsValid (true)) {
                return str;
            }

            if (others != null) {
                return others.FirstOrDefault (x => x.IsValid (true));
            }

            return string.Empty;
        }

        #region 转换成其他类型

        /// <summary>
        /// 转换成Boolean类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">转换失败时返回的默认值</param>
        /// <returns></returns>
        public static bool ToBoolean (this string str, bool defaultValue = false) {
            return str.TryBoolean () ?? defaultValue;
        }

        /// <summary>
        /// 尝试转换成Boolean类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool? TryBoolean (this string str) {
            try {
                return Convert.ToBoolean (str);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 转换成Int32类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">转换失败时返回的默认值</param>
        /// <returns></returns>
        public static int ToInt32 (this string str, int defaultValue = 0) {
            return str.TryInt32 () ?? defaultValue;
        }

        /// <summary>
        /// 尝试转换成Int32类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int? TryInt32 (this string str) {
            try {
                return Convert.ToInt32 (str);
            } catch (Exception ex) {
                return null;
            }
        }

        public static uint ToUint32 (this string str, uint defaultValue = 0) {
            return str.TryUint32 () ?? defaultValue;
        }

        /// <summary>
        /// 尝试转换成Uint32类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static uint? TryUint32 (this string str) {
            try {
                return Convert.ToUInt32 (str);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 转换成Long类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">转换失败时返回的默认值</param>
        /// <returns></returns>
        public static long ToLong (this string str, long defaultValue = 0) {
            return str.TryLong () ?? defaultValue;
        }

        /// <summary>
        /// 尝试转换成Long类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long? TryLong (this string str) {
            try {
                return Convert.ToInt64 (str);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 转换成Double类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">转换失败时返回的默认值</param>
        /// <returns></returns>
        public static double ToDouble (this string str, double defaultValue = 0.0) {
            return str.TryDouble () ?? defaultValue;
        }

        /// <summary>
        /// 尝试转换成Double类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double? TryDouble (this string str) {
            try {
                return Convert.ToDouble (str);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 转换成Decimal类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">转换失败时返回的默认值</param>
        /// <returns></returns>
        public static decimal ToDecimal (this string str, decimal defaultValue = 0.0m) {
            return str.TryDecimal () ?? defaultValue;
        }

        /// <summary>
        /// 尝试转换成Decimal类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal? TryDecimal (this string str) {
            try {
                return Convert.ToDecimal (str);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 转换成DateTime类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="format">日期格式</param>
        /// <returns></returns>
        public static DateTime ToDateTime (this string str, string format = null) {
            return str.TryDateTime () ?? DateTime.MinValue;
        }

        /// <summary>
        /// 转换成DateTime类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">转换失败时返回的默认值</param>
        /// <param name="format">日期格式</param>
        /// <returns></returns>
        public static DateTime ToDateTime (this string str, DateTime defaultValue, string format = null) {
            return str.TryDateTime () ?? defaultValue;
        }

        /// <summary>
        /// 尝试转换成DateTime类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="format">日期格式</param>
        /// <returns></returns>
        public static DateTime? TryDateTime (this string str, string format = null) {
            try {
                if (!format.IsValid (true)) {
                    return Convert.ToDateTime (str);
                }

                return DateTime.ParseExact (str, format, CultureInfo.CurrentCulture);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 转换成
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encodingName"></param>
        /// <returns></returns>
        public static byte[] ToBytes (this string str, string encodingName = "UTF-8") {
            try {
                if (!str.IsValid ()) {
                    return null;
                }

                return Encoding.GetEncoding (encodingName)?.GetBytes (str);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 获取枚举类型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetEnum (this string str, Type type) {
            try {
                return Enum.Parse (type, str);
            } catch (Exception ex) {
                return null;
            }
        }
        #endregion

        #region 重复添加字符或字符串
        /// <summary>
        /// 在字符串左侧重复添加特定次数的特定字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="repeatChar">要添加的字符</param>
        /// <param name="repeatCount">重复添加的次数</param>
        /// <returns></returns>
        public static string RepeatLeft (this string str, char repeatChar, int repeatCount) =>
            str.Repeat ((sb) => sb.Append (repeatChar), repeatCount, false);

        /// <summary>
        /// 在字符串左侧重复添加特定次数的特定字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="repeatWord">要添加的字符串</param>
        /// <param name="repeatCount">重复添加的次数</param>
        /// <returns></returns>
        public static string RepeatLeft (this string str, string repeatWord, int repeatCount) =>
            str.Repeat ((sb) => sb.Append (repeatWord), repeatCount, false);

        /// <summary>
        /// 在字符串右侧重复添加特定次数的特定字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="repeatChar">要添加的字符</param>
        /// <param name="repeatCount">重复添加的次数</param>
        /// <returns></returns>
        public static string RepeatRight (this string str, char repeatChar, int repeatCount) =>
            str.Repeat ((sb) => sb.Append (repeatChar), repeatCount, true);

        /// <summary>
        /// 在字符串右侧重复添加特定次数的特定字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="repeatWord">要添加的字符串</param>
        /// <param name="repeatCount">重复添加的次数</param>
        /// <returns></returns>
        public static string RepeatRight (this string str, string repeatWord, int repeatCount) =>
            str.Repeat ((sb) => sb.Append (repeatWord), repeatCount, true);

        /// <summary>
        /// 重复添加操作
        /// </summary>
        /// <param name="str"></param>
        /// <param name="repeatAction"></param>
        /// <param name="repeatCount"></param>
        /// <param name="isRight"></param>
        /// <returns></returns>
        private static string Repeat (this string str, Action<StringBuilder> repeatAction, int repeatCount, bool isRight) {
            if (!str.IsValid () || repeatCount <= 0) {
                return str;
            }

            var sbResult = new StringBuilder ();

            if (isRight) {
                sbResult.Append (str);
            }

            for (; repeatCount > 0; --repeatCount) {
                repeatAction (sbResult);
            }

            if (!isRight) {
                sbResult.Append (str);
            }

            return sbResult.ToString ();
        }
        #endregion

        #region 随机数字或字母
        /// <summary>
        /// 获取随机数字
        /// </summary>
        /// <param name="length">随机长度</param>
        /// <param name="prefix">前缀</param>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public static string GetRandNumber (int length, string prefix = null, string suffix = null) {
            var buffer = new StringBuilder ();

            if (prefix.IsValid ()) {
                buffer.Append (prefix.GetValue ());
            }

            var maxIndex = NumberSeeds.Length - 1;

            for (var i = 0; i < length; ++i) {
                var rand = new Random (Guid.NewGuid ().GetHashCode ());
                var index = rand.Next (0, maxIndex);
                buffer.Append (NumberSeeds[index]);
            }

            return buffer.ToString ();
        }

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="length">随机长度</param>
        /// <param name="prefix">前缀</param>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public static string GetRandString (int length, string prefix = null, string suffix = null) {
            var buffer = new StringBuilder ();

            if (prefix.IsValid ()) {
                buffer.Append (prefix.GetValue ());
            }

            var maxIndex = AllSeeds.Length - 1;

            for (var i = 0; i < length; ++i) {
                var rand = new Random (Guid.NewGuid ().GetHashCode ());
                var index = rand.Next (0, maxIndex);
                buffer.Append (AllSeeds[index]);
            }

            return buffer.ToString ();
        }

        /// <summary>
        ///
        /// </summary>
        static readonly string NumberSeeds = "1234567890";

        /// <summary>
        ///
        /// </summary>
        static readonly string AllSeeds = NumberSeeds + "aAbBcCdDeFfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ";
        #endregion

        #region StringBuilder扩展
        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value">要追加的字符</param>
        /// <param name="separator">分隔字符</param>
        /// <returns></returns>
        public static StringBuilder Append (this StringBuilder sb, string value, char separator) {
            if (null == sb || !value.IsValid ()) {
                return sb;
            }

            if (sb.Length > 0) {
                sb.Append (separator);
            }

            sb.Append (value);

            return sb;
        }

        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value">要追加的字符</param>
        /// <param name="separator">分隔字符串</param>
        /// <returns></returns>
        public static StringBuilder Append (this StringBuilder sb, string value, string separator) {
            if (null == sb || !value.IsValid ()) {
                return sb;
            }

            if (sb.Length > 0 && separator.IsValid ()) {
                sb.Append (separator);
            }

            sb.Append (value);

            return sb;
        }
        #endregion

        /// <summary>
        /// 将Json字符转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJson<T> (this string json) {
            try {
                if (json.IsValid ()) {
                    return JsonConvert.DeserializeObject<T> (json);
                }
            } catch (Exception ex) { }

            return default (T);
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lowerRestChars">小写其他字母</param>
        /// <returns></returns>
        public static string UpperFirstChar (this string str, bool lowerRestChars = true) {
            if (!str.IsValid ()) {
                return string.Empty;
            }

            if (lowerRestChars) {
                return $"{str.Substring(0, 1).UpperCase()}{str.Substring(1).LowerCase()}";
            } else {
                return $"{str.Substring(0, 1).UpperCase()}{str.Substring(1)}";
            }
        }

        /// <summary>
        /// 是否为<abc>k__BackingField
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBackField (this string str) {
            if (!str.IsValid ()) {
                return false;
            }

            return str.EndsWith ("k__BackingField");
        }

        /// <summary>
        /// 改变日期时间字符串的显示格式
        /// </summary>
        /// <param name="str"></param>
        /// <param name="format">转换前的格式</param>
        /// <param name="originalFormat">转换后的格式</param>
        /// <returns></returns>
        public static string ChangeDateTimeFormat (this string str, string format, string originalFormat = null) {
            if (!str.IsValid () || !format.IsValid ()) {
                return str.GetValue ();
            }

            var dtTemp = str.TryDateTime (originalFormat);
            if (null == dtTemp) {
                return string.Empty;
            }

            return dtTemp.Value.ToString (format);
        }

        /// <summary>
        /// 获取小数位数
        /// </summary>
        /// <param name="numText"></param>
        /// <returns></returns>
        public static int GetDecimalPlace (this string numText) {
            if (!numText.IsValid ()) {
                return 0;
            }

            string str;
            if (numText.UpperCase ().Contains ("E")) {
                str = decimal.Parse (numText, NumberStyles.Float).ToString ("G");
            } else {
                str = numText.ToDouble ().ToString ("G");
            }

            var dot = str.LastIndexOf ('.');
            if (dot < 0) {
                return 0;
            }

            return str.Substring (dot + 1).Length;
        }

        /// <summary>
        /// 获取url中的host
        /// </summary>
        /// <param name="uriString"></param>
        /// <param name="retPort">同时返回端口号（忽略80端口）</param>
        /// <returns></returns>
        public static string GetUriHost (this string uriString, bool retPort = true) {
            if (!uriString.IsValid ()) {
                return string.Empty;
            }

            try {
                if (uriString.IndexOf (NET_PROCOTOL_ROOT) < 0) {
                    uriString = uriString.GetUriString ();
                }

                var uri = new Uri (uriString);
                if (!retPort || uri.Port == 80) {
                    return uri.Host;
                }

                return $"{uri.Host}:{uri.Port}";
            } catch (Exception ex) {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取完整的uri
        /// </summary>
        /// <param name="host"></param>
        /// <param name="protocol">协议名称</param>
        /// <returns></returns>
        public static string GetUriString (this string host, string protocol = NET_PROCOTOL_HTTP) {
            if (!host.IsValid ()) {
                return string.Empty;
            }

            var prefix = protocol.GetValue (NET_PROCOTOL_HTTP).Replace (NET_PROCOTOL_ROOT, string.Empty);
            prefix = $"{prefix}{NET_PROCOTOL_ROOT}";

            var nakedHost = host;
            var index = host.IndexOf (NET_PROCOTOL_ROOT);
            if (index >= 0) {
                nakedHost = host.Substring (index + NET_PROCOTOL_ROOT.Length);
            }

            return $"{prefix}{nakedHost}";
        }

        private const string NET_PROCOTOL_ROOT = "://";
        private const string NET_PROCOTOL_HTTP = "http";
    }
}