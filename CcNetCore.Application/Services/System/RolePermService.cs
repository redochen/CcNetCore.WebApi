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
    /// 角色权限服务
    /// </summary>
    public class RolePermService : SysService<RolePermDto, RolePermission>, IRolePermService, ITransientInstance {
        //自动装载属性（必须为public，否则自动装载失败）
        public new IRolePermRepository _Repo { get; set; }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Result Save (int userID, SaveRolePermDto dto) {
            if (null == dto || (dto.RoleCodes.IsEmpty () && dto.PermCodes.IsEmpty ())) {
                return ErrorCode.InvalidParam.ToResult ();
            }

            if (dto.RoleCodes.IsEmpty ()) {
                return _Repo.DeleteIn ("PermCode", dto.PermCodes).ToResult ();
            }

            Result result = null;

            dto.RoleCodes.ForEach (r => {
                result = SaveRolePerms (userID, r, dto.PermCodes);
                return !result.IsSuccess ();
            });

            return result;
        }

        /// <summary>
        /// 保存角色的权限
        /// </summary>
        /// <param name="userID">创建用户ID</param>
        /// <param name="roleCode">要保存权限的角色编码</param>
        /// <param name="permCodes">要保存的权限编码集合</param>
        /// <returns></returns>
        private Result SaveRolePerms (int userID, string roleCode, string[] permCodes) {
            if (!roleCode.IsValid ()) {
                return ErrorCode.InvalidParam.ToResult ();
            }

            //先获取该角色的全部权限
            var (_, existsItems, ex) = _Repo.Select (new RolePermission { RoleCode = roleCode });
            if (ex != null) {
                return ex.ToResult ();
            }

            var existsPerms = existsItems?.Select (x => x.PermCode);

            //保存新增的权限
            var addPerms = permCodes?.Except (existsPerms);
            if (!addPerms.IsEmpty ()) {
                ex = _Repo.Add (addPerms.Select (x => {
                    var entity = new RolePermission {
                    RoleCode = roleCode,
                    PermCode = x
                    };

                    HandleCreateEntity (userID, entity);
                    return entity;
                }));
                if (ex != null) {
                    return ex.ToResult ();
                }
            }

            //删除移除的权限
            var delPerms = permCodes.IsEmpty () ? existsPerms : existsPerms?.Except (permCodes);
            if (!delPerms.IsEmpty ()) {
                var uids = existsItems.Where (x => delPerms.Any (y => y.Equals (x.PermCode)))
                    .Select (x => x.Uid);

                return _Repo.DeleteIn ("Uid", uids).ToResult ();
            }

            return ErrorCode.Success.ToResult ();
        }
    }
}