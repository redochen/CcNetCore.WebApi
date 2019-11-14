using CcNetCore.Application.Interfaces;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 用户服务
    /// </summary>
    public class UserService : SysService<UserDto, User>, IUserService, ITransientInstance {
        /// <summary>
        /// 处理创建Dto
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="entity"></param>
        protected override void HandleCreateEntity (int userID, User entity) {
            base.HandleCreateEntity (userID, entity);

            entity.UserType = entity.UserType ?? UserType.General;
        }
    }
}