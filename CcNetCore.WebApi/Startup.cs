using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Helpers;
using CcNetCore.WebApi.Controllers;
using CcNetCore.WebApi.Utils;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Swashbuckle.AspNetCore.Swagger;

namespace CcNetCore.WebApi {
    public class Startup {
        public IConfigurationRoot Configuration { get; }
        public static ILoggerRepository LoggerRepository { get; set; }
        public static IAppSettings AppSettings { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="env"></param>
        public Startup (IHostingEnvironment env) {
            var builder = new ConfigurationBuilder ()
                .SetBasePath (env.ContentRootPath)
                .AddJsonFile ("appsettings.json", optional : true, reloadOnChange : true)
                .AddJsonFile ($"appsettings.{env.EnvironmentName}.json", optional : true)
                .AddEnvironmentVariables ();

            this.Configuration = builder.Build ();

            #region log4net配置
            LoggerRepository = LogManager.CreateRepository ("NETCoreRepository");

            // 指定配置文件
            XmlConfigurator.Configure (LoggerRepository, new FileInfo ("log4net.config"));
            #endregion

            AppSettings = ConfigHelper.GetOption<AppSettingsOption> (Configuration, nameof (AppSettings));
        }

        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices (IServiceCollection services) {
            //替换系统默认Controller创建器，必须放到AddMvc之前
            services.Replace (ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator> ());

            services.AddSession (options => {
                options.IdleTimeout = TimeSpan.FromMinutes (AppSettings.TokenExpireMinutes);
            });

            services.AddMvc (options => {
                options.Filters.Add (typeof (AuthFilter));
                options.Filters.Add (typeof (LogFilter));
            }).SetCompatibilityVersion (CompatibilityVersion.Version_2_2);

            //使用自动映射
            services.UseAutoMapper ();

            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen (options => {
                options.SwaggerDoc ("v1", new Info { Title = "CC开发者API", Version = "v1" });
            });

            return IocManager.Instance.Initialize (services, GetAllAssemblyTypes (), RegisterControllers);
        }

        /// <summary>
        /// 配置HTTP请求管道
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }

            app.UseSession ();
            app.UseHttpsRedirection ();
            app.UseAuthentication ();
            app.UseMvc ();

            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger ();

            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI (options => {
                options.SwaggerEndpoint ("/swagger/v1/swagger.json", "CC开发者API V1");
            });
        }

        /// <summary>
        /// 获取所有程序集中的类型
        /// </summary>
        /// <returns></returns>
        private List<Type> GetAllAssemblyTypes () {
            //所有程序集 和程序集下类型
            var libs = DependencyContext.Default.CompileLibraries.Where (l => !l.Serviceable && l.Type != "package"); //排除所有的系统程序集、Nuget下载包
            var types = new List<Type> ();
            foreach (var lib in libs) {
                try {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName (new AssemblyName (lib.Name));
                    types.AddRange (assembly.GetTypes ().Where (t => t != null));
                } catch { }
            }

            return types;
        }

        private void RegisterControllers (ContainerBuilder builder, IEnumerable<Type> types) {
            //注册日志拦截器
            //builder.RegisterType<LogInterceptor> ();

            //注册Controller,实现属性注入
            var controllerType = typeof (IApiController);
            var controllerTypes = types.Where (t => controllerType.IsAssignableFrom (t) &&
                t != controllerType).ToArray ();

            builder.RegisterTypes (controllerTypes)
                .AsSelf ()
                .PropertiesAutowired ();
            //.EnableInterfaceInterceptors () //接口或类需要为public或virtual
            //.EnableClassInterceptors()
            //.InterceptedBy (typeof (LogInterceptor));
        }
    }
}