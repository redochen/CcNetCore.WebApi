using CcNetCore.Utils.Extensions;

namespace CcNetCore.Utils {
    /// <summary>
    /// 验证类型
    /// </summary>
    public enum ValidationType : uint {
        /// <summary>
        /// 输入
        /// </summary>
        Input = 0x1,

        /// <summary>
        /// 输出
        /// </summary>
        Import = 0x2,

        /// <summary>
        /// 所有
        /// </summary>
        All = Input | Import,
    }

    /// <summary>
    /// ValidationType扩展类
    /// </summary>
    public static class ValidationTypeExtension {
        /// <summary>
        /// 检查当前标志中是否包含目标标志组
        /// </summary>
        /// <param name="self"></param>
        /// <param name="flags">目标标志组</param>
        /// <param name="checkAll">检查全部或任意</param>
        /// <returns></returns>
        public static bool Contains (this ValidationType self, ValidationType flags, bool checkAll = true) {
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
        public static ValidationType AddFlags (this ValidationType self, ValidationType flags) {
            self = (ValidationType) ((uint) self).AddFlags ((uint) flags);
            return self;
        }

        /// <summary>
        /// 从前当标志中排除目标标志组
        /// </summary>
        /// <param name="self"></param>
        /// <param name="flags">目标标志组</param>
        /// <returns></returns>
        public static ValidationType RemoveFlags (this ValidationType self, ValidationType flags) {
            self = (ValidationType) ((uint) self).RemoveFlags ((uint) flags);
            return self;
        }
    }
}