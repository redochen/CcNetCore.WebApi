using System;
using System.Security.Claims;
using CcNetCore.Application;
using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Utils;
using CcNetCore.WebApi.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 身份认证接口
    /// </summary>
    [Route ("api/oauth")]
    [ApiController]
    public class OauthController : ControllerBase, IApiController {
        //自动装载属性（必须为public，否则自动装载失败）
        public IAccountService _Account { get; set; }

        /// <summary>
        /// Cookie选项
        /// </summary>
        /// <value></value>
        private CookieOptions _CookieOptions = new CookieOptions {
            HttpOnly = true,
            Secure = false,
            Expires = DateTime.Now.AddMinutes (Startup.AppSettings.TokenExpireMinutes),
        };

        /// <summary>
        /// 身份认证
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet ("auth")]
        public IResult Auth (string username, string password) {
            var verfiyResult = _Account.Verify (new VerifyUserDto {
                UserName = username,
                    PasswordHash = password
            });

            if (!verfiyResult.IsSuccess ()) {
                if (verfiyResult.Exception is NotFoundException) {
                    return Results.InvalidIdentity;
                } else {
                    return verfiyResult;
                }
            }

            var user = verfiyResult.Data;
            if (null == user) {
                return Results.InternalError;
            }

            if (user.Status == Status.Forbidden) {
                return Results.ForbiddenAccount;
            }

            if (user.IsLocked ?? false) {
                return Results.LockedAccount;
            }

            var claimsIdentity = new ClaimsIdentity (new Claim[] {
                new Claim (ClaimTypes.NameIdentifier, username),
                    new Claim (nameof (user.UserName), user.UserName),
                    new Claim (nameof (user.UserID), user.UserID.ToString ()),
                    new Claim (nameof (user.Uid), user.Uid),
                    new Claim (nameof (user.NickName), user.NickName),
                    new Claim (nameof (user.UserType), ((int) user.UserType).ToString ()),
                    new Claim (nameof (user.Avatar), user.Avatar),
            });

            var token = JwtBearerAuthentication.GetJwtAccessToken (claimsIdentity);

            //HttpContext.Session.SetString (Constants.KEY_ACCESS_TOKEN, token);
            HttpContext.Response.Cookies.Append (Constants.KEY_ACCESS_TOKEN, token, _CookieOptions);

            var result = ErrorCode.Success.ToResult<Result<string>> ();
            result.Data = token;
            return result;
        }
    }
}