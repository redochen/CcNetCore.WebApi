using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 用户服务
    /// </summary>
    public class UserService : BaseService<UserModel, UserDto>, IUserService, ITransientInstance {
        //自动装载属性（必须为public，否则自动装载失败）
        public new IUserRepository _Repo { get; set; }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResult ChangePwd (ChangePwdModel model) =>
            _Repo.ChangePassword (GetDto (model), model.NewPasswordHash).ToResult ();

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<UserModel> Verify (VerifyUserModel model) {
            var (user, ex) = _Repo.VerifyUser (GetDto (model));
            var result = ex.ToResult<Result<UserModel>> ();
            result.Data = GetModel (user);
            return result;
        }

        /// <summary>
        /// 处理创建Dto
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="dto"></param>
        protected override void HandleCreateDto (int userID, UserDto dto) {
            base.HandleCreateDto (userID, dto);

            dto.UserType = dto.UserType ?? UserType.General;
        }
    }
}