using CcNetCore.Application.Models;

namespace CcNetCore.Application.Interfaces {
    /// <summary>
    /// 用户角色服务接口
    /// </summary>
    public interface IUserRoleService : IService<UserRoleModel> {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        BaseResult Save (int userID, SaveUserRoleModel model);
    }
}