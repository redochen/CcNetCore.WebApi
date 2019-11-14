using CcNetCore.Application.Models;
using CcNetCore.Domain.Dtos;

namespace CcNetCore.Application.Interfaces {
    /// <summary>
    /// 角色权限服务接口
    /// </summary>
    public interface IRolePermService : ISysService<RolePermDto> {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        Result Save (int userID, SaveRolePermDto model);
    }
}