using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.WebApi.Utils {
    /// <summary>
    /// 授权过滤器
    /// </summary>
    public class AuthFilter : IAuthorizationFilter {
        private ITokenService _Token => IocManager.Instance.Resolve<ITokenService> ();

        public void OnAuthorization (AuthorizationFilterContext context) {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor) {
                var anonymous = descriptor.MethodInfo.GetCustomAttributes (inherit: true)
                    .Any (a => a.GetType ().Equals (typeof (AnonymousAttribute)));
                if (anonymous) {
                    return;
                }
            }

            //先从Session中取，再从Cookie中取（防止服务重启导致Session清空）
            var token = context.HttpContext?.Session?.GetString (Constants.SESSION_KEY_TOKEN);
            if (!token.IsValid ()) {
                context.HttpContext?.Request?.Cookies?.TryGetValue (
                    Constants.SESSION_KEY_TOKEN, out token);
            }

            var errCode = _Token.CheckToken (token, out int userID);
            if (errCode != ErrorCode.Success) {
                context.Result = new JsonResult (errCode.ToResult ());
                return;
            }

            context.RouteData.Values.Add (Constants.ROUTE_DATA_KEY_USER_ID, userID);
        }
    }

    /// <summary>
    /// 匿名访问属性
    /// </summary>
    public class AnonymousAttribute : ActionFilterAttribute { }
}