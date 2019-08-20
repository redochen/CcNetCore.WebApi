using System;
using System.Text.RegularExpressions;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Helpers;

namespace CcNetCore.Utils.Attributes {
    /// <summary>
    /// 字符验证属性
    /// </summary>
    [AttributeUsage (AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class StringValidationAttribute : BaseValidationAttribute {
        /// <summary>
        /// 是否允许空白字符
        /// </summary>
        public bool AllowSpace { get; set; } = false;

        /// <summary>
        /// 正则表达式语句
        /// </summary>
        public string RegexPattern { get; set; }

        /// <summary>
        /// 正则表达式选项
        /// </summary>
        public RegexOptions RegexOptions { get; set; } = RegexOptions.IgnoreCase;

        /// <summary>
        /// 正则匹配失败时的信息
        /// </summary>
        public string RegExMessage { get; set; }

        /// <summary>
        /// 日期时间格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">属性或字段的名称</param>
        public StringValidationAttribute (string name) : base (name) {

        }

        /// <summary>
        /// 验证是否有效
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid (object value) {
            if (!base.IsValid (value)) {
                return false;
            }

            var str = value?.ToString ();
            if (string.IsNullOrEmpty (str)) {
                if (!AllowEmpty) {
                    ErrorMessage = $"\"{Name.GetValue()}\"不允许空值";
                    return false;
                }
                return true;
            }

            if (string.IsNullOrWhiteSpace (str)) {
                if (!AllowSpace) {
                    ErrorMessage = $"\"{Name.GetValue()}\"不允许空白字符";
                    return false;
                }
                return true;
            }

            if (RegexPattern.IsValid ()) {
                var rgx = new Regex (RegexPattern, RegexOptions);
                if (!rgx.IsMatch (value?.ToString ())) {
                    ErrorMessage = $"\"{Name.GetValue()}\"{RegExMessage.GetValue("格式错误")}";
                    return false;
                }
            }

            CustomFormat format = null;
            if (DateTimeFormat.IsValid ()) {
                format = CustomFormat.DateTimeFormat (DateTimeFormat);
                var error = InputHelper.ValidateInput (TextInputMode.All, value?.ToString ().GetValue (), false, format);
                if (error.IsValid ()) {
                    ErrorMessage = $"\"{Name.GetValue()}\"{error}";
                    return false;
                }
            }

            return true;
        }
    }
}