using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CcNetCore.Utils.Helpers {
    /// <summary>
    /// 配置帮助类
    /// </summary>
    public class ConfigHelper {
        private IConfigurationRoot Configuration { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="file"></param>
        public ConfigHelper (string file) {
            var builder = new ConfigurationBuilder ();
            builder.AddJsonFile (file, false, false);
            Configuration = builder.Build ();
        }

        /// <summary>
        /// 获取设置选项
        /// </summary>
        /// <param name="section">设置节名称</param>
        /// <typeparam name="T"></typeparam>
        public T GetOption<T> (string section) where T : class, new () =>
            GetOption<T> (Configuration, section);

        /// <summary>
        /// 获取设置选项
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="section">设置节名称</param>
        /// <typeparam name="T"></typeparam>
        public static T GetOption<T> (IConfiguration configuration, string section)
        where T : class, new () =>
            GetOption<T> (new ServiceCollection (), configuration, section);

        /// <summary>
        /// 获取设置选项
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="section">设置节名称</param>
        /// <typeparam name="T"></typeparam>
        public static T GetOption<T> (IServiceCollection services,
            IConfiguration configuration, string section) where T : class, new () {
            var option = services.AddOptions ()
                .Configure<T> (configuration.GetSection (section))
                .BuildServiceProvider ()
                .GetService<IOptions<T>> ()
                .Value;
            return option;
        }
    }
}