using System;
using System.ComponentModel.DataAnnotations;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.Utils.Attributes {
    /// <summary>
    /// 基本验证属性
    /// </summary>
    [AttributeUsage (AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class BaseValidationAttribute : ValidationAttribute {
        /// <summary>
        /// 属性或字段的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 验证类型
        /// </summary>
        public ValidationType Type { get; set; } = ValidationType.Input;

        /// <summary>
        /// 是否允许空值（或null）
        /// </summary>
        public bool AllowEmpty { get; set; } = false;

        /// <summary>
        /// 最小长度（<=0表示不限制）
        /// </summary>
        public int MinLength { get; set; } = 0;

        /// <summary>
        /// 最大长度（<=0表示不限制）
        /// </summary>
        public int MaxLength { get; set; } = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">属性或字段的名称</param>
        public BaseValidationAttribute (string name) : base () {
            Name = name;
        }

        /// <summary>
        /// 验证是否有效
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid (object value) {
            if (!AllowEmpty && null == value) {
                ErrorMessage = $"\"{Name.GetValue()}\"不允许空值";
                return false;
            }

            var length = value?.ToString ()?.GetValue ()?.Length ?? 0;
            if (length == 0 && AllowEmpty) {
                return true;
            }

            if (MinLength > 0 && length < MinLength) {
                ErrorMessage = $"\"{Name.GetValue()}\"不足最小长度 {MinLength}";
                return false;
            }

            if (MaxLength > 0 && length > MaxLength) {
                ErrorMessage = $"\"{Name.GetValue()}\"超过最大长度 {MaxLength}";
                return false;
            }

            return true;
        }
    }
}