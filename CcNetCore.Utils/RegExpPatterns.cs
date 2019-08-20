namespace CcNetCore.Utils {
    /// <summary>
    // 正则表达式集合
    /// </summary>
    public static class RegExpPatterns {
        /// <summary>
        /// 国内手机
        /// </summary>
        public const string Mobile = @"^0?(13|14|15|17|18|19)[0-9]{9}$";

        /// <summary>
        /// 国内固定电话
        /// </summary>
        public const string Telephone = @"^[0-9-()（）]{7,13}$";

        /// <summary>
        /// 电子邮件
        /// </summary>
        public const string Email = @"^([\w\-\.])+\@([\w\-\.])+\.([a-z]{2,4})$";

        /// <summary>
        /// 整数数字
        /// </summary>
        public const string IntegerNumbers = @"^(0|(-?[1-9]\d*))$";

        /// <summary>
        /// 浮点数字
        /// </summary>
        public const string FloatNumbers = @"^-?(([1-9]\d*\.\d*)|(0\.\d*[1-9]\d*))$";

        /// <summary>
        /// 英文字母或数字
        /// </summary>
        public const string NumbersAndChars = @"^[a-z\d]+$";

        /// <summary>
        /// 特殊字符
        /// </summary>
        private const string SpecialChars = @"`~!@#\$\^&%\*()=|""_\+{}':;,\[\].\\<>/\?！￥……（）——{}【】‘；：”“'。，、？";

        /// <summary>
        /// 排特殊字符
        /// </summary>
        public const string NoSpecialChars = @"^[^`~!@#\$\^&%\*()=|""_\+{}':;,\[\].\\<>/\?！￥……（）——{}【】‘；：”“'。，、？]*$";
    }
}