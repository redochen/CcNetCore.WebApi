using CcNetCore.Application.Models;
using CcNetCore.Domain.Dtos;

namespace CcNetCore.Application.Interfaces {
    /// <summary>
    /// 用户角色服务接口
    /// </summary>
    public interface IUserRoleService : ISysService<UserRoleDto> {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        Result Save (int userID, SaveUserRoleDto model);
    }
}