using System;
using System.Data;
using System.Linq;
using CcNetCore.Domain.Entities;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Interfaces;
using Dapper.Contrib.Extensions;

namespace CcNetCore.Infrastructure.Repositories {
    /// <summary>
    /// 角色权限仓储类
    /// </summary>
    public class RolePermRepository : EntityRepository<RolePermission>, IRolePermRepository, ISingletonInstance {
        /// <summary>
        /// 查询已存在的数据项
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="rolePerm">要保存的数据项</param>
        /// <returns></returns>
        protected override (RolePermission, Exception) QueryExists (IDbConnection conn, RolePermission rolePerm) {
            if (null == rolePerm || !rolePerm.Uid.IsValid () ||
                !rolePerm.RoleCode.IsValid () || !rolePerm.PermCode.IsValid ()) {
                return (null, Exceptions.InvalidParam);
            }

            var matchFields = new string[] {
                nameof (rolePerm.Uid), nameof (rolePerm.RoleCode), nameof (rolePerm.PermCode)
            };

            var matchSql = $"{{0}} {LogicTypes.OR} ({{1}} {LogicTypes.AND} {{2}})";
            var predicate = new SqlPredicate<RolePermission> (rolePerm, matchSql, matchFields);

            var (_, items) = conn.GetWhere (predicate);
            return (items?.FirstOrDefault (), null);
        }

        /// <summary>
        /// 更新已存在的数据项
        /// </summary>
        /// <param name="exists">已存在的数据项</param>
        /// <param name="rolePerm">要保存的数据项</param>
        protected override void UpdateExists (RolePermission exists, RolePermission rolePerm) {
            exists.RoleCode = rolePerm.RoleCode.GetValue (exists.RoleCode);
            exists.PermCode = rolePerm.PermCode.GetValue (exists.PermCode);
        }
    }
}