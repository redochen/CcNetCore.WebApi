using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CcNetCore.Common;
using CcNetCore.WebApi.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CcNetCore.WebApi.Extensions {
    /// <summary>
    /// JWT授权扩展
    /// </summary>
    public static class JwtBearerAuthentication {
        private static IAppSettings _appSettings;

        /// <summary>
        /// 注册JWT Bearer认证服务的静态扩展方法
        /// </summary>
        /// <param name="services"></param>
        /// <param name="appSettings">JWT授权的配置项</param>
        public static void AddJwtBearerAuthentication (this IServiceCollection services, IAppSettings appSettings) {
            _appSettings = appSettings;

            //使用应用密钥得到一个加密密钥字节数组
            var key = Encoding.ASCII.GetBytes (appSettings.Secret);

            services.AddAuthentication (options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie (options => options.SlidingExpiration = true)
                .AddJwtBearer (options => {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.Events = new JwtBearerEvents {
                        OnMessageReceived = (context) => {
                            context.HttpContext.Request.Cookies.TryGetValue (Constants.KEY_ACCESS_TOKEN, out string token);
                            context.Token = token;

                            return Task.CompletedTask;
                        }
                    };
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey (key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }

        public static string GetJwtAccessToken (ClaimsIdentity claimsIdentity) {
            var key = Encoding.ASCII.GetBytes (_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes (_appSettings.TokenExpireMinutes),
                SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler ();
            var token = tokenHandler.CreateToken (tokenDescriptor);
            return tokenHandler.WriteToken (token);
        }
    }
}