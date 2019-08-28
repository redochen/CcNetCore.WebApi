using System.Linq;
using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 用户角色服务
    /// </summary>
    public class UserRoleService : BaseService<UserRoleModel, UserRoleDto>, IUserRoleService, ITransientInstance {
        //自动装载属性（必须为public，否则自动装载失败）
        public new IUserRoleRepository _Repo { get; set; }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResult Save (int userID, SaveUserRoleModel model) {
            //先获取该用户的全部角色
            var (existsItems, ex) = _Repo.Query (
                new UserRoleDto { UserGuid = model.UserGuid });
            if (ex != null) {
                return ex.ToResult ();
            }

            var existsRoles = existsItems?.Select (x => x.RoleCode);

            //保存新增的角色
            var addRoles = model.RoleCodes?.Except (existsRoles);
            if (!addRoles.IsEmpty ()) {
                ex = _Repo.Add (addRoles.Select (x => {
                    var dto = new UserRoleDto {
                    UserGuid = model.UserGuid,
                    RoleCode = x
                    };

                    HandleCreateDto (userID, dto);
                    return dto;
                }));
                if (ex != null) {
                    return ex.ToResult ();
                }
            }

            //删除移除的角色
            var delRoles = existsRoles?.Except (model.RoleCodes);
            if (!delRoles.IsEmpty ()) {
                var uids = existsItems.Where (x => delRoles.Any (y => y.Equals (x.RoleCode)))
                    .Select (x => x.Uid);

                return DeleteByUids (uids, null);
            }

            return ErrorCode.Success.ToResult ();
        }
    }
}