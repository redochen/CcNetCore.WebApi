using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.WebApi {
    /// <summary>
    /// 自动映射设置
    /// </summary>
    public static class AutoMappings {
        /// <summary>
        /// 使用自动映射
        /// </summary>
        /// <param name="services"></param>
        public static void UseAutoMapper (this IServiceCollection services) {
            var assemblies = Assembly.GetEntryAssembly ()
                .GetAssignableAssemblies<IAutoMapperProfile> (includeSelf: true);
            services.AddAutoMapper (assemblies);
        }
    }
}