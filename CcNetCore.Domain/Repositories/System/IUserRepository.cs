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
        /// <returns></returns>
        (UserDto, Exception) VerifyUser (UserDto user);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newPasswordHash">新密码哈希值</param>
        /// <returns></returns>
        Exception ChangePassword (UserDto user, string newPasswordHash);
    }
}