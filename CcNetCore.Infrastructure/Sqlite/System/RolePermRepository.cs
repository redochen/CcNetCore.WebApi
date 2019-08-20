using System;
using System.Data;
using System.Linq;
using Dapper.Contrib.Extensions;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Infrastructure.Sqlite.System {
    /// <summary>
    /// 角色权限仓储类
    /// </summary>
    public class RolePermRepository : SqliteRepository<RolePermDto, RolePermission>, IRolePermissionRepository, ISingletonInstance {
        /// <summary>
        /// 查询已存在的数据项
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="rolePerm">要保存的数据项</param>
        /// <param name="isCreation">是否为创建，否则为更新</param>
        /// <param name="exists">已存在的数据项</param>
        /// <returns></returns>
        protected override Exception QueryExists (IDbConnection conn, RolePermission rolePerm, bool isCreation, out RolePermission exists) {
            exists = null;

            if (null == rolePerm || !rolePerm.Uid.IsValid () ||
                !rolePerm.RoleCode.IsValid () || !rolePerm.PermCode.IsValid ()) {
                return Exceptions.InvalidParam;
            }

            var matchFields = new string[] {
                nameof (rolePerm.Uid), nameof (rolePerm.RoleCode), nameof (rolePerm.PermCode)
            };

            var matchSql = $"{{0}} {MatchSql.OR} ({{1}} {MatchSql.AND} {{2}})";
            exists = conn.GetWhere (rolePerm, matchSql, matchFields)?.FirstOrDefault ();

            return null;
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