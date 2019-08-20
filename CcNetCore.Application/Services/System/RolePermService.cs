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
    /// 角色权限服务
    /// </summary>
    public class RolePermService : BaseService<RolePermModel, RolePermDto>, IRolePermService, ITransientInstance {
        //自动装载属性（必须为public，否则自动装载失败）
        public new IRolePermissionRepository _Repo { get; set; }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResult Save (int userID, SaveRolePermModel model) {
            //先获取该角色的全部权限
            var (existsItems, ex) = _Repo.Query (
                new RolePermDto { RoleCode = model.RoleCode });
            if (ex != null) {
                return ex.ToResult ();
            }

            var existsPerms = existsItems?.Select (x => x.PermCode);

            //保存新增的权限
            var addPerms = model.PermCodes?.Except (existsPerms);
            if (!addPerms.IsEmpty ()) {
                ex = _Repo.Add (addPerms.Select (x => {
                    var dto = new RolePermDto {
                    RoleCode = model.RoleCode,
                    PermCode = x
                    };

                    HandleCreateDto (userID, dto);
                    return dto;
                }));
                if (ex != null) {
                    return ex.ToResult ();
                }
            }

            //删除移除的权限
            var delPerms = existsPerms?.Except (model.PermCodes);
            if (!delPerms.IsEmpty ()) {
                var uids = existsItems.Where (x => delPerms.Any (y => y.Equals (x.PermCode)))
                    .Select (x => x.Uid);

                ex = BatchDelete ("Uid", uids);
                if (ex != null) {
                    return ex.ToResult ();
                }
            }

            return ErrorCode.Success.ToResult ();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResult Delete (DeleteRolePermModel model) =>
            BatchDelete ("Uid", model.Uid).ToResult ();
    }
}