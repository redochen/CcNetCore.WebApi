using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CcNetCore.Domain.Entities;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Interfaces;
using Dapper;
using Dapper.Contrib.Extensions;

namespace CcNetCore.Infrastructure.Repositories {
    /// <summary>
    /// 账户仓储类
    /// </summary>
    public class AccountRepository : DapperRepository, IAccountRepository, ISingletonInstance {
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public (User, Exception) VerifyUser (User user) {
            if (null == user || !user.UserName.IsValid () || !user.PasswordHash.IsValid ()) {
                return (null, Exceptions.InvalidParam);
            }

            var matchFields = new string[] {
                nameof (user.UserName), nameof (user.PasswordHash), nameof (user.IsDeleted)
            };

            //未删除的用户
            user.IsDeleted = 0;

            var predicate = new SqlPredicate<User> (user, matchFields);
            var (_, items, ex) = Select (predicate);
            if (items.IsEmpty ()) {
                return (null, ex ?? Exceptions.NotFound);
            }

            return (items.First (), ex);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newPasswordHash">新密码哈希值</param>
        /// <returns></returns>
        public Exception ChangePassword (User user, string newPasswordHash) =>
            _Dapper.Update (user, updateFunc: (conn, trans, entity) =>
                ChangePassword (conn, trans, entity, newPasswordHash));

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="user"></param>
        /// <param name="newPasswordHash">新密码哈希值</param>
        /// <returns></returns>
        private Exception ChangePassword (IDbConnection conn, IDbTransaction trans, User user, string newPasswordHash) {
            if (null == user || !user.Uid.IsValid () ||
                !user.PasswordHash.IsValid () || !newPasswordHash.IsValid ()) {
                return Exceptions.InvalidParam;
            }

            var matchFields = new string[] {
                nameof (user.Uid), nameof (user.IsDeleted)
            };

            //未删除的用户
            user.IsDeleted = 0;

            var predicate = new SqlPredicate<User> (user, matchFields);
            var (_, items, ex) = Select (predicate);
            var item = items?.FirstOrDefault (x => x != null);
            if (null == item) {
                return Exceptions.NotFound;
            }

            if (!user.PasswordHash.Equals (item.PasswordHash)) {
                return Exceptions.Indentify;
            }

            item.PasswordHash = newPasswordHash;
            item.UpdateUser = user.UpdateUser;
            item.UpdateTime = DateTime.Now;

            return conn.Update (item, trans) ? null : Exceptions.Failure;
        }

        /// <summary>
        /// 获取用户权限列表
        /// </summary>
        /// <param name="userUid">用户UID</param>
        /// <returns></returns>
        public (IEnumerable<UserPermission>, Exception) GetUserPerms (string userUid) {
            var sql = @"SELECT DISTINCT
                            ur.user_uid,
                            p.perm_code,
                            p.action_code,
                            p.perm_name,
                            p.perm_type,
                            m.menu_name,
                            m.uid AS menu_uid,
                            m.menu_alias,
                            m.menu_url,
                            m.is_default AS is_default_menu
                        FROM sys_user_roles ur
                        LEFT JOIN sys_role_perms rp ON rp.role_code = ur.role_code
                        AND rp.del_flag = 0
                        AND rp.status = 1
                        INNER JOIN sys_perms p ON p.perm_code = rp.perm_code
                        AND p.del_flag = 0
                        AND p.status = 1
                        INNER JOIN sys_menus m ON m.uid = p.menu_uid
                        AND m.del_flag = 0
                        AND m.status = 1
                        WHERE ur.del_flag = 0
                        AND ur.user_uid = @Uid
                        ORDER BY p.id;";
            var dyncParms = new DynamicParameters ();
            dyncParms.Add ("@Uid", userUid);

            var (_, items, ex) = Select<UserPermission> (sql, dyncParms);
            return (items, ex);
        }

        /// <summary>
        /// 获取用户菜单列表
        /// </summary>
        /// <param name="userUid">用户UID</param>
        /// <returns></returns>
        public (IEnumerable<Menu>, Exception) GetUserMenus (string userUid) {
            var sql = @"SELECT DISTINCT m.*
                        FROM sys_user_roles ur
                        LEFT JOIN sys_role_perms rp ON rp.role_code = ur.role_code
                        AND rp.del_flag = 0
                        AND rp.status = 1
                        INNER JOIN sys_perms p ON p.perm_code = rp.perm_code
                        AND p.del_flag = 0
                        AND p.status = 1
                        INNER JOIN sys_menus m ON m.uid = p.menu_uid
                        AND m.del_flag = 0
                        AND m.status = 1
                        WHERE ur.del_flag = 0
                        AND ur.user_uid = @Uid
                        ORDER BY p.id;";
            var dyncParms = new DynamicParameters ();
            dyncParms.Add ("@Uid", userUid);

            var (_, items, ex) = Select<Menu> (sql, dyncParms);
            return (items, ex);
        }
    }
}