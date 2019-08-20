using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.Utils.Helpers {
    /// <summary>
    /// 文本输入帮助类
    /// </summary>
    public static class InputHelper {
        /// <summary>
        /// 检查输入
        /// </summary>
        /// <param name="mode">输入模式</param>
        /// <param name="keyChar">正在输入的字符</param>
        /// <param name="funcGetText">获取已输入内容的回调</param>
        /// <param name="isCasual">GridView请传入True，其他请传入False</param>
        /// <returns>成功或失败</returns>
        public static bool CheckInput (TextInputMode mode, char keyChar, Func<string> funcGetText, bool isCasual = false) {
            //控制字符总是允许输入
            if (IsControlChar (keyChar)) {
                return true;
            }

            switch (mode) {
                case TextInputMode.Characters:
                    return AllowInputCharacters (keyChar);
                case TextInputMode.Digitals:
                    return AllowInputDigitals (keyChar);
                case TextInputMode.UnsignedInt:
                    return AllowInputNumbers (keyChar, false, false, funcGetText, isCasual);
                case TextInputMode.SignedInt:
                    return AllowInputNumbers (keyChar, true, false, funcGetText, isCasual);
                case TextInputMode.UnsignedFloat:
                    return AllowInputNumbers (keyChar, false, true, funcGetText, isCasual);
                case TextInputMode.SignedFloat:
                    return AllowInputNumbers (keyChar, true, true, funcGetText, isCasual);
                case TextInputMode.All:
                default:
                    return true;
            }
        }

        /// <summary>
        /// 是否允许字符
        /// </summary>
        /// <param name="keyChar">正在输入的字符</param>
        /// <returns></returns>
        public static bool AllowInputCharacters (char keyChar) {
            return ((keyChar >= ChrALower && keyChar <= ChrZLower) ||
                (keyChar >= ChrAUpper && keyChar <= ChrZUpper));
        }

        /// <summary>
        /// 是否允许数字
        /// </summary>
        /// <param name="keyChar">正在输入的字符</param>
        /// <returns></returns>
        public static bool AllowInputDigitals (char keyChar) {
            return (keyChar >= ChrZero && keyChar <= ChrNine);
        }

        /// <summary>
        /// 是否允许数字
        /// </summary>
        /// <param name="keyChar">正在输入的字符</param>
        /// <param name="isSigined">是否为有符号整数</param>
        /// <param name="allowFloat">是否允许输入小数</param>
        /// <param name="funcGetText">获取已输入内容的回调</param>
        /// <param name="isCasual">GridView请传入True，其他请传入False</param>
        /// <returns></returns>
        public static bool AllowInputNumbers (char keyChar, bool isSigined, bool allowFloat, Func<string> funcGetText, bool isCasual = false) {
            if (keyChar >= ChrZero && keyChar <= ChrNine) {
                return true;
            }

            var content = funcGetText?.Invoke ()?.GetValue ();

            if (keyChar == ChrMinus && isSigined) {
                if (isCasual) {
                    return true;
                }

                //已经存在，则不允许输入
                if (content.ToCharArray ().Contains (ChrMinus)) {
                    return false;
                }

                return true;

                //只能是第一个字符
                //return !content.IsValid();
            }

            if (keyChar == ChrDot && allowFloat) {
                if (!content.IsValid ()) {
                    return isCasual;
                }

                //已经存在，则不允许输入
                if (content.ToCharArray ().Contains (ChrDot)) {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 验证输入
        /// </summary>
        /// <param name="mode">输入模式</param>
        /// <param name="inputText">输入的内容</param>
        /// <param name="allowZero">是否允许零值</param>
        /// <param name="format">格式化实例</param>
        /// <param name="extValidate">额外的验证方法</param>
        /// <returns>错误信息</returns>
        public static string ValidateInput (TextInputMode mode, string inputText, bool allowZero,
            CustomFormat format = null, Func<string, string> extValidate = null) {
            if (!inputText.IsValid () /* || !InputRegexes.ContainsKey(mode)*/ ) {
                return string.Empty;
            }

            //自定义格式校验
            if (!format?.Validating (inputText) ?? false) {
                return format.ErrorInfos.FirstOrDefault ()?.GetValue (InvalidFormat);
            }

            if (!allowZero) {
                var value = inputText.TryDouble ();
                if (value.HasValue && value == 0.0D) {
                    return "不允许输入零值";
                }
            }

            //预定义格式校验
            var rgx = GetValidatingRegex (mode, true);
            if (rgx != null && !rgx.IsMatch (inputText)) {
                return mode.GetDesc ();
            }

            return extValidate != null ? extValidate (inputText) : string.Empty;
        }

        /// <summary>
        /// 获取验证的正则表达式实例
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="finalCheck">是否为最终数据验证（输入过程中的验证请传入False）</param>
        /// <returns></returns>
        public static Regex GetValidatingRegex (TextInputMode mode, bool finalCheck) {
            var rgxes = finalCheck ? FinalCheckRegexes : InputCheckRegexes;
            return rgxes.ContainsKey (mode) ? rgxes[mode] : null;
        }

        public const char ChrDot = '.';
        public const char ChrMinus = '-';
        public const char ChrZero = '0';
        public const char ChrNine = '9';
        public const char ChrALower = 'a';
        public const char ChrZLower = 'z';
        public const char ChrAUpper = 'A';
        public const char ChrZUpper = 'Z';

        /// <summary>
        /// 是否为控制字符
        /// </summary>
        /// <param name="keyChar">正在输入的字符</param>
        /// <param name="checkControlGroupKeys">是否检查Ctrl组合键</param>
        /// <returns></returns>
        public static bool IsControlChar (char keyChar, bool checkControlGroupKeys = true) {
            return (ControlChars.Contains (keyChar) || (checkControlGroupKeys && ControlGroupKeys.Contains (keyChar)));
        }

        private readonly static int[] ControlChars = {
            8 /*BackSpace*/ ,
            9 /*Tab*/ ,
            13 /*Enter*/ ,
            16 /*Shift*/ ,
            17 /*Control*/ ,
            18 /*Alt*/ ,
            20 /*Cape Lock*/ ,
        };

        private readonly static int[] ControlGroupKeys = {
            1 /*Ctrl+A*/ ,
            3 /*Ctrl+C*/ ,
            22 /*Ctrl+V*/ ,
            26 /*Ctrl+Z*/ ,
        };

        //public readonly static Keys KeysCommit = Keys.Enter;
        //public readonly static Keys KeysSelect = Keys.Space;
        //public readonly static Keys[] KeysHorizontal = { Keys.Left, Keys.Right, Keys.Tab };
        //public readonly static Keys[] KeysVertical = { Keys.Up, Keys.Down };

        public readonly static Regex RgxCharacters = new Regex (@"^[a-z]*$", RegexOptions.IgnoreCase);
        public readonly static Regex RgxDigitals = new Regex (@"^\d*$");
        public readonly static Regex RgxUnsignedInt = new Regex (@"^(([1-9]\d+)|\d)$");

        public readonly static Regex RgxSignedInt_Input = new Regex (@"^-?(([1-9]\d+)|\d)$");
        public readonly static Regex RgxUnsignedFloat_Input = new Regex (@"^(([1-9]\d+)|\d)?\.?\d*$");
        public readonly static Regex RgxSignedFloat_Input = new Regex (@"^-?(([1-9]\d+)|\d)?\.?\d*$");

        public readonly static Regex RgxSignedInt_Final = new Regex (@"^-?(([1-9]\d+)|\d)$");
        public readonly static Regex RgxUnsignedFloat_Final = new Regex (@"^(([1-9]\d+)|\d)(\.\d+)?$");
        public readonly static Regex RgxSignedFloat_Final = new Regex (@"^-?(([1-9]\d+)|\d)(\.\d+)?$");

        private readonly static Dictionary<TextInputMode, Regex> InputCheckRegexes =
            new Dictionary<TextInputMode, Regex> () {
                [TextInputMode.Characters] = RgxCharacters, [TextInputMode.Digitals] = RgxDigitals, [TextInputMode.UnsignedInt] = RgxUnsignedInt, [TextInputMode.SignedInt] = RgxSignedInt_Input, [TextInputMode.UnsignedFloat] = RgxUnsignedFloat_Input, [TextInputMode.SignedFloat] = RgxSignedFloat_Input,
            };

        private readonly static Dictionary<TextInputMode, Regex> FinalCheckRegexes =
            new Dictionary<TextInputMode, Regex> () {
                [TextInputMode.Characters] = RgxCharacters, [TextInputMode.Digitals] = RgxDigitals, [TextInputMode.UnsignedInt] = RgxUnsignedInt, [TextInputMode.SignedInt] = RgxSignedInt_Final, [TextInputMode.UnsignedFloat] = RgxUnsignedFloat_Final, [TextInputMode.SignedFloat] = RgxSignedFloat_Final,
            };

        /// <summary>
        ///
        /// </summary>
        public const string InvalidFormat = "输入格式错误";
    }

    /// <summary>
    /// 文本输入模式
    /// </summary>
    public enum TextInputMode {
        /// <summary>
        /// 允许所有输入
        /// </summary>
        All = 0,

        [Description ("只能输入字符")]
        Characters,

        [Description ("只能输入数字")]
        Digitals,

        [Description ("只能输入正整数")]
        UnsignedInt,

        [Description ("只能输入整数")]
        SignedInt,

        [Description ("只能输入正整数或小数")]
        UnsignedFloat,

        [Description ("只能输入数字")]
        SignedFloat,
    }
}