using System;
using System.Collections.Generic;
using CcNetCore.Domain.Entities;

namespace CcNetCore.Domain.Repositories {
    /// <summary>
    /// 账户仓储接口
    /// </summary>
    public interface IAccountRepository {
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        (User, Exception) VerifyUser (User user);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newPasswordHash">新密码哈希值</param>
        /// <returns></returns>
        Exception ChangePassword (User user, string newPasswordHash);

        /// <summary>
        /// 获取用户权限列表
        /// </summary>
        /// <param name="userUid">用户UID</param>
        /// <returns></returns>
        (IEnumerable<UserPermission>, Exception) GetUserPerms (string userUid);

        /// <summary>
        ///获取用户菜单列表
        /// </summary>
        /// <param name="usrUid">用户UID</param>
        /// <returns></returns>
        (IEnumerable<Menu>, Exception) GetUserMenus (string usrUid);
    }
}