using CcNetCore.Application.Models;
using CcNetCore.Domain.Dtos;

namespace CcNetCore.Application.Interfaces {
    /// <summary>
    /// 账户服务接口
    /// </summary>
    public interface IAccountService {
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Result ChangePwd (ChangePwdDto dto);

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Result<UserDto> Verify (VerifyUserDto dto);

        /// <summary>
        /// 获取用户权限列表
        /// </summary>
        /// <param name="userUid">用户UID</param>
        /// <returns></returns>
        ListResult<UserPermDto> GetUserPerms (string userUid);

        /// <summary>
        ///获取用户菜单列表
        /// </summary>
        /// <param name="usrUid">用户UID</param>
        /// <returns></returns>
        ListResult<MenuDto> GetUserMenus (string usrUid);
    }
}