namespace CcNetCore.Utils.Extensions {
    /// <summary>
    /// 位操作扩展类
    /// </summary>
    public static class BitExtension {
        /// <summary>
        /// 测试当前标志中是否包含目标标志组中的任意一个
        /// </summary>
        /// <param name="current">当前标志</param>
        /// <param name="flags">目标标志组</param>
        /// <returns></returns>
        public static bool ContainsAny (this uint current, uint flags) {
            return (current & flags) != 0;
        }

        /// <summary>
        /// 测试当前标志中是否包含目标标志组的所有
        /// </summary>
        /// <param name="current">当前标志</param>
        /// <param name="flags">目标标志组</param>
        /// <returns></returns>
        public static bool ContainsAll (this uint current, uint flags) {
            return (current & flags) == flags;
        }

        /// <summary>
        /// 将目标标志组添加到当前标志中
        /// </summary>
        /// <param name="current"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static uint AddFlags (this uint current, uint flags) {
            current |= flags;
            return current;
        }

        /// <summary>
        /// 从前当标志中排除目标标志组
        /// </summary>
        /// <param name="current"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static uint RemoveFlags (this uint current, uint flags) {
            current &= ~flags;
            return current;
        }
    }
}