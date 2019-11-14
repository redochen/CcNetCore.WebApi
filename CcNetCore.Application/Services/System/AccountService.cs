using System.Linq;
using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 账户服务
    /// </summary>
    public class AccountService : BaseService, IAccountService, ITransientInstance {
        //自动装载属性（必须为public，否则自动装载失败）
        public IAccountRepository _Repo { get; set; }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Result ChangePwd (ChangePwdDto dto) =>
            _Repo.ChangePassword (GetEntity<User> (dto), dto.NewPasswordHash).ToResult ();

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Result<UserDto> Verify (VerifyUserDto dto) {
            var (user, ex) = _Repo.VerifyUser (GetEntity<User> (dto));
            var result = ex.ToResult<Result<UserDto>> ();
            result.Data = GetDto<UserDto> (user);
            return result;
        }

        /// <summary>
        /// 获取用户权限列表
        /// </summary>
        /// <param name="userUid">用户UID</param>
        /// <returns></returns>
        public ListResult<UserPermDto> GetUserPerms (string userUid) {
            var (items, ex) = _Repo.GetUserPerms (userUid);
            var result = ex.ToResult<ListResult<UserPermDto>> ();
            result.Items = items?.Select (x => GetDto<UserPermDto> (x))?.ToList ();
            return result;
        }

        /// <summary>
        /// 获取用户菜单列表
        /// </summary>
        /// <param name="userUid">用户UID</param>
        /// <returns></returns>
        public ListResult<MenuDto> GetUserMenus (string userUid) {
            var (items, ex) = _Repo.GetUserMenus (userUid);
            var result = ex.ToResult<ListResult<MenuDto>> ();
            result.Items = items?.Select (x => GetDto<MenuDto> (x))?.ToList ();
            return result;
        }
    }
}