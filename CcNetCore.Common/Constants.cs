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
        ///
        /// </summary>
        public const string PREFIX_CHECK_PERMISSION = "CP|";

        /// <summary>
        /// 权限代码长度
        /// </summary>
        public const int RAND_LEN_PERMISSION_CODE = 8;

        /// <summary>
        /// 键名之访问令牌
        /// </summary>
        public const string KEY_ACCESS_TOKEN = "access_token";

        /// <summary>
        /// 路由数据之请求ID
        /// /// </summary>
        public const string ROUTE_DATA_KEY_REQ_ID = "req_rand";

        /// <summary>
        /// 空UID
        /// </summary>
        public const string UID_EMPTY = "0";
    }
}