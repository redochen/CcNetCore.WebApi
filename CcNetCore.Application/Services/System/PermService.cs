using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 权限服务
    /// </summary>
    public class PermService : BaseService<PermModel, PermDto>, IPermService, ITransientInstance {
        /// <summary>
        /// 处理创建Dto
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="dto"></param>
        protected override void HandleCreateDto (int userID, PermDto dto) {
            base.HandleCreateDto (userID, dto);

            dto.Type = dto.Type ?? PermType.Menu;
            dto.Code = StringExtension.GetRandString (
                Constants.RAND_LEN_PERMISSION_CODE, Constants.RAND_PREFIX_PERMISSION_CODE
            );
        }
    }
}