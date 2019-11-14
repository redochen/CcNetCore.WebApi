using System.Collections.Generic;
using System.Linq;
using CcNetCore.Application;
using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;
using CcNetCore.WebApi.Services;
using CcNetCore.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 账号接口
    /// </summary>
    [Route ("api/account")]
    [ApiController]
    public class AccountController : BaseController, IApiController {
        #region 自动装载属性（必须为public，否则自动装载失败）
        public IUserService _User { get; set; }
        public IAccountService _Account { get; set; }
        public IMenuService _Menu { get; set; }
        #endregion

        /// <summary>
        /// 修改登录密码
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("change_pwd")]
        public Result ChangePwd ([FromBody] ChangePwdDto dto) =>
            HandleRequest<Result> ((userID) => {
                var result = _Account.ChangePwd (dto);
                if (result.Exception is IdentityException) {
                    result.Message = "密码错误";
                }

                return result;
            });

        /// <summary>
        /// 获取用户资料及权限列表
        /// </summary>
        /// <returns></returns>
        [HttpGet ("profile")]
        public IResult Profile () {
            var userUid = AuthContextService.CurrentUser?.Uid;
            if (!userUid.IsValid ()) {
                return Results.NeedLogin;
            }

            var userRes = _User.Get (userUid);
            if (!userRes.IsSuccess ()) {
                return userRes.ToResult ();
            }

            var userPermsRes = _Account.GetUserPerms (userUid);
            if (!userPermsRes.IsSuccess ()) {
                return userPermsRes.ToResult ();
            }

            var permissions = userPermsRes.Items?.GroupBy (x => x.MenuAlias)
                .ToDictionary (g => g.Key, g => g.Select (x => x.ActionCode).Distinct ());

            var result = ErrorCode.Success.ToResult<Result<object>> ();
            result.Data = new {
                access = new string[] { },
                avatar = userRes.Data.Avatar,
                user_guid = userRes.Data.Uid,
                user_name = userRes.Data.NickName,
                user_type = userRes.Data.UserType,
                permissions = permissions
            };

            return result;
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet ("menu")]
        public IActionResult Menus () {
            var userUid = AuthContextService.CurrentUser?.Uid;
            if (!userUid.IsValid ()) {
                return Ok (Results.NeedLogin);
            }

            List<MenuDto> menus = null;

            var isSuperAdmin = (UserType.SuperAdmin == AuthContextService.CurrentUser.UserType);
            if (!isSuperAdmin) {
                var userMenuRes = _Account.GetUserMenus (userUid);
                if (!userMenuRes.IsSuccess ()) {
                    return Ok (userMenuRes.ToResult ());
                }

                menus = userMenuRes.Items;
            }

            var allMenuRes = _Menu.Get (new MenuDto { Status = Status.Normal });
            if (!allMenuRes.IsSuccess ()) {
                return Ok (allMenuRes.ToResult ());
            }

            var rootMenus = allMenuRes.Items?.Where (m => (!m.ParentUid.IsValid () || Constants.UID_EMPTY == m.ParentUid));

            if (isSuperAdmin) {
                menus = allMenuRes.Items;
            } else {
                foreach (var root in rootMenus) {
                    if (!menus.Exists (x => x.Uid == root.Uid)) {
                        menus.Add (root);
                    }
                }
            }

            menus = menus.OrderBy (x => x.Sort).ThenBy (x => x.CreateTime).ToList ();
            menus.ForEach (m => m.ParentUid = m.ParentUid.GetValue (Constants.UID_EMPTY));

            var menuItems = menus.LoadMenuItems (Constants.UID_EMPTY);

            return Ok (menuItems);
        }
    }
}