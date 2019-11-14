using System;
using System.Data;
using System.Linq;
using CcNetCore.Domain.Entities;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Infrastructure.Repositories {
    /// <summary>
    /// 权限仓储类
    /// </summary>
    public class PermissionRepository : EntityRepository<Permission>, IPermissionRepository, ISingletonInstance {
        /// <summary>
        /// 查询已存在的数据项
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="perm">要保存的数据项</param>
        /// <returns></returns>
        protected override (Permission, Exception) QueryExists (IDbConnection conn, Permission perm) {
            if (!perm.Name.IsValid () || !perm.Code.IsValid ()) {
                return (null, Exceptions.InvalidParam);
            }

            var matchFields = new string[] {
                nameof (perm.Uid), nameof (perm.Name), nameof (perm.Code)
            };

            var (_, items, ex) = Select (perm, matchFields);
            return (items?.FirstOrDefault (), ex);
        }

        /// <summary>
        /// 更新已存在的数据项
        /// </summary>
        /// <param name="exists">已存在的数据项</param>
        /// <param name="perm">要保存的数据项</param>
        protected override void UpdateExists (Permission exists, Permission perm) {
            exists.Name = perm.Name.GetValue (exists.Name);
            exists.Type = perm.Type ?? exists.Type;
            exists.MenuGuid = perm.MenuGuid.GetValue (exists.MenuGuid);
            exists.ActionCode = perm.ActionCode.GetValue (exists.ActionCode);
            exists.Icon = perm.Icon.GetValue (exists.Icon);
            exists.Description = perm.Description.GetValue (exists.Description);
        }
    }
}