using System;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Helpers;

namespace CcNetCore.Utils.Attributes {
    /// <summary>
    /// 数字验证属性
    /// </summary>
    [AttributeUsage (AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class NumberValidationAttribute : BaseValidationAttribute {
        /// <summary>
        /// 是否允许0值
        /// </summary>
        public bool AllowZero { get; set; } = false;

        /// <summary>
        /// 是否允许小数值
        /// </summary>
        public bool AllowDot { get; set; } = false;

        /// <summary>
        /// 是否为有符号数字
        /// </summary>
        public bool IsSigned { get; set; } = true;

        /// <summary>
        /// 小数位数（<=0表示不限制）
        /// </summary>
        public uint DecimalPlace { get; set; } = 0;

        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue { get; set; } = double.MinValue;

        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue { get; set; } = double.MaxValue;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">属性或字段的名称</param>
        public NumberValidationAttribute (string name) : base (name) { }

        /// <summary>
        /// 验证是否有效
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid (object value) {
            if (!base.IsValid (value)) {
                return false;
            }

            var mode = TextInputMode.All;
            if (AllowDot) //小数
            {
                mode = IsSigned ? TextInputMode.SignedFloat : TextInputMode.UnsignedFloat;
            } else //整数
            {
                mode = IsSigned ? TextInputMode.SignedInt : TextInputMode.UnsignedInt;
            }

            CustomFormat format = null;
            double? minValue = (MinValue > double.MinValue) ? MinValue : (double?) null;
            double? maxValue = (MaxValue < double.MaxValue) ? MaxValue : (double?) null;

            if (DecimalPlace > 0 || minValue.HasValue || maxValue.HasValue) {
                format = CustomFormat.DecimalFormat (
                    decimalPlace: (DecimalPlace > 0 ? (uint?) DecimalPlace : null),
                    minValue: (minValue.HasValue ? (decimal?) minValue.Value : null),
                    maxValue: (maxValue.HasValue ? (decimal?) maxValue.Value : null));
            }

            ErrorMessage = InputHelper.ValidateInput (mode, value?.ToString ().GetValue (), AllowZero, format);
            if (ErrorMessage.IsValid ()) {
                if (DecimalPlace > 0 && ErrorMessage.Contains (InputHelper.InvalidFormat)) {
                    ErrorMessage = $"\"{Name.GetValue()}\"最多只能输入 {DecimalPlace} 位小数";
                } else {
                    ErrorMessage = $"\"{Name.GetValue()}\"{ErrorMessage}";
                }

                return false;
            }

            if (!AllowEmpty && !(value?.ToString ().IsValid () ?? false)) {
                ErrorMessage = $"\"{Name.GetValue()}\"不能为空";
                return false;
            }

            return true;
        }
    }
}