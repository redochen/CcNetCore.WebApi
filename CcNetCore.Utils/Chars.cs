namespace CcNetCore.Utils {
    /// <summary>
    /// 字符集类
    /// </summary>
    public static class Chars {
        public static readonly ConstChar 空格 = ' ';
        public static readonly ConstChar 换行符 = '\n';
        public static readonly ConstChar 波浪符 = '~';
        public static readonly ConstChar 顿号 = '`';
        public static readonly ConstChar 感叹号 = '!';
        public static readonly ConstChar 邮箱 = '@';
        public static readonly ConstChar 井号 = '#';
        public static readonly ConstChar 美元符 = '$';
        public static readonly ConstChar 百分符 = '%';
        public static readonly ConstChar 尖号 = '^';
        public static readonly ConstChar 和号 = '&';
        public static readonly ConstChar 星号 = '*';
        public static readonly ConstChar 左小括符 = '(';
        public static readonly ConstChar 右小括符 = ')';
        public static readonly ConstChar 下划线 = '_';
        public static readonly ConstChar 减号 = '-';
        public static readonly ConstChar 加号 = '+';
        public static readonly ConstChar 等号 = '=';
        public static readonly ConstChar 左中括符 = '[';
        public static readonly ConstChar 右中括符 = ']';
        public static readonly ConstChar 左大括符 = '{';
        public static readonly ConstChar 右大括符 = '}';
        public static readonly ConstChar 竖线 = '|';
        public static readonly ConstChar 反斜线 = '\\';
        public static readonly ConstChar 冒号 = ':';
        public static readonly ConstChar 分号 = ';';
        public static readonly ConstChar 双引号 = '"';
        public static readonly ConstChar 单引号 = '\'';
        public static readonly ConstChar 问号 = '?';
        public static readonly ConstChar 正斜线 = '/';
        public static readonly ConstChar 逗号 = ',';
        public static readonly ConstChar 句号 = '.';
        public static readonly ConstChar 左尖括符 = '<';
        public static readonly ConstChar 右尖括符 = '>';
    }

    /// <summary>
    /// 字符常量类
    /// </summary>
    public class ConstChar {
        private readonly char Char;
        private readonly string Str;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="chr"></param>
        public ConstChar (char chr) {
            Char = chr;
            Str = new string (chr, 1);
        }

        /// <summary>
        /// 赋值操作符
        /// </summary>
        /// <param name="chr"></param>
        public static implicit operator ConstChar (char chr) => new ConstChar (chr);

        /// <summary>
        /// 强制类型转换
        /// </summary>
        /// <param name="chr"></param>
        public static implicit operator char (ConstChar chr) => chr.Char;

        /// <summary>
        /// 强制类型转换
        /// </summary>
        /// <param name="chr"></param>
        //public static implicit operator string(ConstChar chr)
        //    => chr.Str;

        public override string ToString () => Str;
    }
}