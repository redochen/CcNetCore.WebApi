namespace CcNetCore.Domain {
    /// <summary>
    /// 连接库上下文接口
    /// </summary>
    public interface IDbContext {
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <returns></returns>
        string GetConnectionString ();
    }
}