using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 角色服务
    /// </summary>
    public class RoleService : BaseService<RoleModel, RoleDto>, IRoleService, ITransientInstance {
        /// <summary>
        /// 处理创建Dto
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="dto"></param>
        protected override void HandleCreateDto (int userID, RoleDto dto) {
            base.HandleCreateDto (userID, dto);

            dto.IsSuperAdmin = dto.IsSuperAdmin ?? false;
            dto.IsBuiltin = dto.IsBuiltin ?? false;
            dto.Code = StringExtension.GetRandString (
                Constants.RAND_LEN_ROLE_CODE, Constants.RAND_PREFIX_ROLE_CODE);
        }
    }
}