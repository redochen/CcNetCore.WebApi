using CcNetCore.WebApi.Utils;
using Microsoft.AspNetCore.Builder;

namespace CcNetCore.WebApi.Extensions {
    /// <summary>
    /// 自定义异常中间件
    /// </summary>
    public static class CustomExceptionMiddleware {
        public static void UseCustomExceptionMiddleware (this IApplicationBuilder app) {
            app.UseMiddleware<ExceptionMiddleware> ();
        }
    }
}