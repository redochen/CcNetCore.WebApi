namespace CcNetCore.Common {
    /// <summary>
    /// 常量类
    /// </summary>
    public static class Constants {
        /// <summary>
        /// 程序设置文件
        /// </summary>
        public const string FILE_APP_SETTINGS = "appsettings.json";

        /// <summary>
        /// 本地缓存文件
        /// </summary>
        public const string FILE_TOKEN_CACHE = "tokens.dat";

        /// <summary>
        /// 令牌随机数前缀
        /// </summary>
        public const string RAND_PREFIX_TOKEN = "TK|";

        /// <summary>
        /// 令牌随机数长度
        /// </summary>
        public const int RAND_LEN_TOKEN = 8;

        /// <summary>
        /// 角色代码前缀
        /// </summary>
        public const string RAND_PREFIX_ROLE_CODE = "RC|";

        /// <summary>
        /// 角色代码长度
        /// </summary>
        public const int RAND_LEN_ROLE_CODE = 8;

        /// <summary>
        /// 权限代码前缀
        /// </summary>
        public const string RAND_PREFIX_PERMISSION_CODE = "PC|";

        /// <summary>
        /// 权限代码长度
        /// </summary>
        public const int RAND_LEN_PERMISSION_CODE = 8;

        /// <summary>
        /// session键名之令牌
        /// </summary>
        public const string SESSION_KEY_TOKEN = "token";

        /// <summary>
        /// 路由数据之请求ID
        /// </summary>
        public const string ROUTE_DATA_KEY_REQ_ID = "req_rand";

        /// <summary>
        /// 路由数据之用户ID
        /// </summary>
        public const string ROUTE_DATA_KEY_USER_ID = "user_id";

        /// <summary>
        /// 路由数据之计时器
        /// </summary>
        public const string ROUTE_DATA_KEY_TIMER = "timer";
    }
}