using CcNetCore.Common;
using CcNetCore.Domain;
using CcNetCore.Utils.Helpers;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.WebApi.Utils {
    /// <summary>
    /// 设置接口
    /// </summary>
    public interface IAppSettings {
        /// <summary>
        /// 密钥
        /// </summary>
        /// <value></value>
        string Secret { get; }

        /// <summary>
        /// 令牌过期分钟数
        /// </summary>
        /// <value></value>
        int TokenExpireMinutes { get; }
    }

    /// <summary>
    /// 程序设置类
    /// </summary>
    public class AppSettingsOption : IAppSettings {
        /// <summary>
        /// 密钥
        /// </summary>
        /// <value></value>
        public string Secret { get; set; }

        /// <summary>
        /// 令牌过期分钟数
        /// </summary>
        /// <value></value>
        public int TokenExpireMinutes { get; set; }
    }

    /// <summary>
    /// 数据库设置类
    /// </summary>
    public class DbSettingsOption {
        /// <summary>
        /// Sqlite数据路径
        /// </summary>
        /// <value></value>
        public string SqliteDB { get; set; }
    }

    /// <summary>
    /// 程序设置类
    /// </summary>
    public class Settings : IAppSettings, IDbContext, ISingletonInstance {
        private ConfigHelper _Config = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Settings () {
            _Config = new ConfigHelper (Constants.FILE_APP_SETTINGS);
            AppSettings = _Config.GetOption<AppSettingsOption> (nameof (AppSettings));
            DbSettings = _Config.GetOption<DbSettingsOption> (nameof (DbSettings));
        }

        /// <summary>
        /// 密钥
        /// </summary>
        /// <value></value>
        public string Secret => AppSettings.Secret;

        /// <summary>
        /// 令牌过期分钟数
        /// </summary>
        /// <value></value>
        public int TokenExpireMinutes => AppSettings.TokenExpireMinutes;

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString () => DbSettings.SqliteDB;

        /// <summary>
        /// 程序设置选项
        /// </summary>
        /// <value></value>
        private AppSettingsOption AppSettings { get; set; }

        /// <summary>
        /// 数据库选项
        /// </summary>
        /// <value></value>
        private DbSettingsOption DbSettings { get; set; }
    }
}