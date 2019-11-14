using System.Security.Claims;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Utils.Extensions;
using Microsoft.AspNetCore.Http;

namespace CcNetCore.WebApi.Services {
    /// <summary>
    /// 授权上下文服务
    /// </summary>
    public class AuthContextService {
        private static IHttpContextAccessor _context;
        /// <summary>
        ///
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public static void Configure (IHttpContextAccessor httpContextAccessor) {
            _context = httpContextAccessor;
        }
        /// <summary>
        ///
        /// </summary>
        public static HttpContext Current => _context.HttpContext;

        /// <summary>
        ///
        /// </summary>
        public static UserDto CurrentUser {
            get {
                var user = new UserDto ();
                user.UserName = Current.User.FindFirstValue (ClaimTypes.NameIdentifier);
                user.UserID = Current.User.FindFirstValue (nameof (user.UserID)).ToInt ();
                user.Uid = Current.User.FindFirstValue (nameof (user.Uid));
                user.NickName = Current.User.FindFirstValue (nameof (user.NickName));
                user.UserType = (UserType) Current.User.FindFirstValue (nameof (user.UserType)).ToInt ();
                user.Avatar = Current.User.FindFirstValue (nameof (user.Avatar));
                return user;
            }
        }

        /// <summary>
        /// 是否已授权
        /// </summary>
        public static bool IsAuthenticated {
            get {
                return Current.User.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public static bool IsSupperAdministator {
            get {
                return ((UserType) Current.User.FindFirstValue ("userType").ToInt () == UserType.SuperAdmin);
            }
        }
    }
}