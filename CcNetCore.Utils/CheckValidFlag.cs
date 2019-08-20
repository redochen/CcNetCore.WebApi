using CcNetCore.Utils.Extensions;

namespace CcNetCore.Utils {
    /// <summary>
    /// 检查有效性标识
    /// </summary>
    public enum CheckValidFlag : uint {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0x0000,

        /// <summary>
        /// 零值视为有效值
        /// </summary>
        ZeroAsValid = 0x0001,

        /// <summary>
        /// 空白字符视为有效值
        /// </summary>
        WhiteSpaceAsValid = 0x0002,

        /// <summary>
        /// DBNull视为有效值
        /// </summary>
        DbNullAsValid = 0x0004,
    }

    /// <summary>
    /// CheckValidFlag扩展类
    /// </summary>
    public static class CheckValidFlagExtension {
        /// <summary>
        /// 检查当前标志中是否包含目标标志组
        /// </summary>
        /// <param name="self"></param>
        /// <param name="flags">目标标志组</param>
        /// <param name="checkAll">检查全部或任意</param>
        /// <returns></returns>
        public static bool Contains (this CheckValidFlag self, CheckValidFlag flags, bool checkAll = true) {
            if (checkAll) {
                return ((uint) self).ContainsAll ((uint) flags);
            } else {
                return ((uint) self).ContainsAny ((uint) flags);
            }
        }

        /// <summary>
        /// 将目标标志组添加到当前标志中
        /// </summary>
        /// <param name="self"></param>
        /// <param name="flags">目标标志组</param>
        /// <returns></returns>
        public static CheckValidFlag AddFlags (this CheckValidFlag self, CheckValidFlag flags) {
            self = (CheckValidFlag) ((uint) self).AddFlags ((uint) flags);
            return self;
        }

        /// <summary>
        /// 从前当标志中排除目标标志组
        /// </summary>
        /// <param name="self"></param>
        /// <param name="flags">目标标志组</param>
        /// <returns></returns>
        public static CheckValidFlag RemoveFlags (this CheckValidFlag self, CheckValidFlag flags) {
            self = (CheckValidFlag) ((uint) self).RemoveFlags ((uint) flags);
            return self;
        }
    }
}