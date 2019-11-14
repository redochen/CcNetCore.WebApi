using CcNetCore.Application.Interfaces;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 权限服务
    /// </summary>
    public class PermService : SysService<PermDto, Permission>, IPermService, ITransientInstance {
        /// <summary>
        /// 处理创建Dto
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="entity"></param>
        protected override void HandleCreateEntity (int userID, Permission entity) {
            base.HandleCreateEntity (userID, entity);

            entity.Type = entity.Type ?? PermType.Menu;
            entity.Code = StringExtension.GetRandString (
                Constants.RAND_LEN_PERMISSION_CODE, Constants.RAND_PREFIX_PERMISSION_CODE
            );
        }
    }
}