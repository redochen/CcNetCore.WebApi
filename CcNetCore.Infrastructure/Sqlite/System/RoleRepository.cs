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

namespace CcNetCore.Infrastructure.Sqlite {
    /// <summary>
    /// 角色仓储类
    /// </summary>
    public class RoleRepository : SqliteRepository<RoleDto, Role>, IRoleRepository, ISingletonInstance {
        /// <summary>
        /// 查询已存在的数据项
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="role">要保存的数据项</param>
        /// <returns></returns>
        protected override (Role, Exception) QueryExists (IDbConnection conn, Role role) {
            if (!role.Name.IsValid () || !role.Code.IsValid ()) {
                return (null, Exceptions.InvalidParam);
            }

            var matchFields = new string[] {
                nameof (role.Uid), nameof (role.Name), nameof (role.Code)
            };

            var exists = conn.GetWhere (role, MatchSql.OR, matchFields)?.FirstOrDefault ();
            return (exists, null);
        }

        /// <summary>
        /// 更新已存在的数据项
        /// </summary>
        /// <param name="exists">已存在的数据项</param>
        /// <param name="role">要保存的数据项</param>
        protected override void UpdateExists (Role exists, Role role) {
            exists.Name = role.Name.GetValue (exists.Name);
            exists.Description = role.Description.GetValue (exists.Description);
            exists.IsSuperAdmin = role.IsSuperAdmin ?? exists.IsSuperAdmin;
            exists.IsBuiltin = role.IsBuiltin ?? exists.IsBuiltin;
        }
    }
}