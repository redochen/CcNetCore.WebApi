using System.Linq;
using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 用户角色服务
    /// </summary>
    public class UserRoleService : SysService<UserRoleDto, UserRole>, IUserRoleService, ITransientInstance {
        //自动装载属性（必须为public，否则自动装载失败）
        public new IUserRoleRepository _Repo { get; set; }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Result Save (int userID, SaveUserRoleDto dto) {
            if (null == dto || (dto.UserGuids.IsEmpty () && dto.RoleCodes.IsEmpty ())) {
                return ErrorCode.InvalidParam.ToResult ();
            }

            if (dto.UserGuids.IsEmpty ()) {
                return _Repo.DeleteIn ("RoleCode", dto.RoleCodes).ToResult ();
            }

            Result result = null;

            dto.UserGuids.ForEach (u => {
                result = SaveUserRoles (userID, u, dto.RoleCodes);
                return !result.IsSuccess ();
            });

            return result;
        }

        /// <summary>
        /// 保存用户的角色
        /// </summary>
        /// <param name="userID">创建用户ID</param>
        /// <param name="userGuid">要保存角色的用户UID</param>
        /// <param name="roleCodes">要保存的角色编码集合</param>
        /// <returns></returns>
        private Result SaveUserRoles (int userID, string userGuid, string[] roleCodes) {
            if (!userGuid.IsValid ()) {
                return ErrorCode.InvalidParam.ToResult ();
            }

            //先获取该用户的全部角色
            var (_, existsItems, ex) = _Repo.Select (new UserRole { UserGuid = userGuid });
            if (ex != null) {
                return ex.ToResult ();
            }

            var existsRoles = existsItems?.Select (x => x.RoleCode);

            //保存新增的角色
            var addRoles = roleCodes?.Except (existsRoles);
            if (!addRoles.IsEmpty ()) {
                ex = _Repo.Add (addRoles.Select (x => {
                    var entity = new UserRole {
                    UserGuid = userGuid,
                    RoleCode = x
                    };

                    HandleCreateEntity (userID, entity);
                    return entity;
                }));
                if (ex != null) {
                    return ex.ToResult ();
                }
            }

            //删除移除的角色
            var delRoles = roleCodes.IsEmpty () ? existsRoles : existsRoles?.Except (roleCodes);
            if (!delRoles.IsEmpty ()) {
                var uids = existsItems.Where (x => delRoles.Any (y => y.Equals (x.RoleCode)))
                    .Select (x => x.Uid);

                return _Repo.DeleteIn ("Uid", uids).ToResult ();
            }

            return ErrorCode.Success.ToResult ();
        }
    }
}