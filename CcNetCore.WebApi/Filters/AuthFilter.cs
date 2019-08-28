using System.Linq;
using CcNetCore.WebApi.Utils;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CcNetCore.WebApi.Filters {
    /// <summary>
    /// 授权过滤器
    /// </summary>
    public class AuthFilter : IAuthorizationFilter {
        //自动装载属性（必须为public，否则自动装载失败）
        public IAppSettings _AppSettings { get; set; }

        //private ITokenService _Token => IocManager.Instance.Resolve<ITokenService> ();

        // https://tpodolak.com/blog/2017/12/13/asp-net-core-memorycache-getorcreate-calls-factory-method-multiple-times/
        //private IMemoryCache _memoryCache;

        public void OnAuthorization (AuthorizationFilterContext context) {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor != null) {
                var anonymous = descriptor.MethodInfo.GetCustomAttributes (inherit: true)
                    .Any (a => a.GetType ().Equals (typeof (AnonymousAttribute)));
                if (anonymous) {
                    return;
                }
            }

            /*
            var user = context.HttpContext.User;
             if (!user.Identity.IsAuthenticated) {
                 throw Exceptions.Unauthorized;
             }

             var key = Constants.PREFIX_CHECK_PERMISSION + AuthContextService.CurrentUser.UserName;
             _memoryCache = (IMemoryCache) context.HttpContext.RequestServices.GetService (typeof (IMemoryCache));

             var permissions = _memoryCache.GetOrCreate<AccessPermissions> (key, (cache) => {
                 cache.SlidingExpiration = TimeSpan.FromMinutes (_AppSettings.TokenExpireMinutes);
                 //TODO: load real permission list from db

                 return new AccessPermissions ();
             });

             var controller = descriptor?.ControllerName;
             var action = descriptor?.ActionName;

             if (!permissions.CanAccess (controller, action)) {
                 throw Exceptions.Unauthorized;
             }
             */
        }
    }

    /// <summary>
    /// 匿名访问属性
    /// </summary>
    public class AnonymousAttribute : ActionFilterAttribute { }
}