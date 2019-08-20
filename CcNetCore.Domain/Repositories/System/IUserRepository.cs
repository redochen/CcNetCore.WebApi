using System;
using CcNetCore.Domain.Dtos;

namespace CcNetCore.Domain.Repositories {
    /// <summary>
    /// 用户仓储接口
    /// </summary>
    public interface IUserRepository : IRepository<UserDto> {
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Exception VerifyUser (UserDto user, out int userID);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newPasswordHash">新密码哈希值</param>
        /// <returns></returns>
        Exception ChangePassword (UserDto user, string newPasswordHash);
    }
}