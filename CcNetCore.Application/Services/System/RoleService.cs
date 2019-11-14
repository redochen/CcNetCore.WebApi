using CcNetCore.Application.Interfaces;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 角色服务
    /// </summary>
    public class RoleService : SysService<RoleDto, Role>, IRoleService, ITransientInstance {
        /// <summary>
        /// 处理创建Dto
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="entity"></param>
        protected override void HandleCreateEntity (int userID, Role entity) {
            base.HandleCreateEntity (userID, entity);

            entity.IsSuperAdmin = entity.IsSuperAdmin ?? false;
            entity.IsBuiltin = entity.IsBuiltin ?? false;
            entity.Code = StringExtension.GetRandString (
                Constants.RAND_LEN_ROLE_CODE, Constants.RAND_PREFIX_ROLE_CODE);
        }
    }
}