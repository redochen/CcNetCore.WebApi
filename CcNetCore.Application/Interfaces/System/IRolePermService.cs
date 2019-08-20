using CcNetCore.Application.Models;

namespace CcNetCore.Application.Interfaces {
    /// <summary>
    /// 角色权限服务接口
    /// </summary>
    public interface IRolePermService : IService<RolePermModel> {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        BaseResult Save (int userID, SaveRolePermModel model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        BaseResult Delete (DeleteRolePermModel model);
    }
}