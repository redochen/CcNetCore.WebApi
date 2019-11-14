using System;
using System.Net;
using System.Threading.Tasks;
using CcNetCore.Application.Models;
using CcNetCore.Utils;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CcNetCore.WebApi.Middlewares {
    /// <summary>
    /// 自定义异常中间件
    /// </summary>
    public static class CustomExceptionMiddleware {
        public static void UseCustomExceptionMiddleware (this IApplicationBuilder app) {
            app.UseMiddleware<ExceptionMiddleware> ();
        }
    }

    /// <summary>
    /// 异常中间件
    /// </summary>
    public class ExceptionMiddleware {
        private ILog _Logger = null;

        private readonly RequestDelegate _next;

        /// <summary>
        ///
        /// </summary>
        /// <param name="next"></param>
        public ExceptionMiddleware (RequestDelegate next) {
            _next = next;
            _Logger = LogManager.GetLogger (Startup.LoggerRepository.Name, this.GetType ());
        }

        public async Task InvokeAsync (HttpContext httpContext) {
            try {
                await _next (httpContext);
            } catch (Exception ex) {
                _Logger.Error (ex.ToString ());
                await HandleExceptionAsync (httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync (HttpContext context, Exception exception) {
            var error = new Result {
                Code = 500,
                Message = $"资源服务器忙,请稍候再试,原因:{exception.Message}"
            };

            if (exception is UnauthorizedException) {
                error.Code = (int) HttpStatusCode.Unauthorized;
                error.Message = "未授权的访问(未登录或者登录已超时)";
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = error.Code;

            return context.Response.WriteAsync (error.ToString ());
        }
    }
}