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
    /// 用户角色仓储类
    /// </summary>
    public class UserRoleRepository : SqliteRepository<UserRoleDto, UserRole>, IUserRoleRepository, ISingletonInstance {
        /// <summary>
        /// 查询已存在的数据项
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userRole">要保存的数据项</param>
        /// <returns></returns>
        protected override (UserRole, Exception) QueryExists (IDbConnection conn, UserRole userRole) {
            if (!userRole.UserGuid.IsValid () || !userRole.RoleCode.IsValid ()) {
                return (null, Exceptions.InvalidParam);
            }

            var matchFields = new string[] {
                nameof (userRole.Uid), nameof (userRole.UserGuid), nameof (userRole.RoleCode)
            };

            var matchSql = $"{{0}} {MatchSql.OR} ({{1}} {MatchSql.AND} {{2}})";
            var exists = conn.GetWhere (userRole, matchSql, matchFields)?.FirstOrDefault ();
            return (exists, null);
        }

        /// <summary>
        /// 更新已存在的数据项
        /// </summary>
        /// <param name="exists">已存在的数据项</param>
        /// <param name="userRole">要保存的数据项</param>
        protected override void UpdateExists (UserRole exists, UserRole userRole) {
            exists.UserGuid = userRole.UserGuid.GetValue (exists.UserGuid);
            exists.RoleCode = userRole.RoleCode.GetValue (exists.RoleCode);
        }
    }
}