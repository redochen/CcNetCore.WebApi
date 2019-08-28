using CcNetCore.Application.Models;

namespace CcNetCore.Application.Interfaces {
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService : IService<UserModel> {
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        BaseResult ChangePwd (ChangePwdModel model);

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<UserModel> Verify (VerifyUserModel model);
    }
}