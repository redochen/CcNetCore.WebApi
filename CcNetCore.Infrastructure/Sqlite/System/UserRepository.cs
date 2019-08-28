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
    /// 用户仓储类
    /// </summary>
    public class UserRepository : SqliteRepository<UserDto, User>, IUserRepository, ISingletonInstance {
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public (UserDto, Exception) VerifyUser (UserDto userDto) {
            var user = GetEntity (userDto);
            if (null == user || !user.UserName.IsValid () || !user.PasswordHash.IsValid ()) {
                return (null, Exceptions.InvalidParam);
            }

            var matchFields = new string[] {
                nameof (user.UserName), nameof (user.PasswordHash), nameof (user.IsDeleted)
            };

            //未删除的用户
            user.IsDeleted = 0;

            var (items, ex) = _Dapper.Query (1, 0, user, MatchSql.AND, matchFields);
            if (items.IsEmpty ()) {
                return (null, ex ?? Exceptions.NotFound);
            }

            return (GetDto (items.First ()), null);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userDto"></param>
        /// <param name="newPasswordHash">新密码哈希值</param>
        /// <returns></returns>
        public Exception ChangePassword (UserDto userDto, string newPasswordHash) =>
            _Dapper.Update (GetEntity (userDto), updateFunc: (conn, trans, user) =>
                ChangePassword (conn, trans, user, newPasswordHash));

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

            var item = conn.GetWhere (user, MatchSql.AND, matchFields)?.FirstOrDefault ();
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
        /// 查询已存在的数据项
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="user">要保存的数据项</param>
        /// <param name="isCreation">是否为创建，否则为更新</param>
        /// <param name="exists">已存在的数据项</param>
        /// <returns></returns>
        protected override Exception QueryExists (IDbConnection conn, User user, bool isCreation, out User exists) {
            exists = null;

            if (null == user || !user.Uid.IsValid ()) {
                return Exceptions.InvalidParam;
            }

            string[] matchFields = null;

            if (isCreation) {
                if (!user.UserName.IsValid () || !user.PasswordHash.IsValid ()) {
                    return Exceptions.InvalidParam;
                }

                matchFields = new string[] {
                    nameof (user.Uid), nameof (user.UserName)
                };
            } else {
                matchFields = new string[] {
                    nameof (user.Uid), nameof (user.IsDeleted)
                };
            }

            exists = conn.GetWhere (user, MatchSql.OR, matchFields)?.FirstOrDefault ();

            return null;
        }

        /// <summary>
        /// 更新已存在的数据项
        /// </summary>
        /// <param name="exists">已存在的数据项</param>
        /// <param name="user">要保存的数据项</param>
        protected override void UpdateExists (User exists, User user) {
            exists.NickName = user.NickName.GetValue (exists.NickName);
            exists.Avatar = user.Avatar.GetValue (exists.Avatar);
            exists.Description = user.Description.GetValue (exists.Description);
            exists.UserType = user.UserType ?? exists.UserType;
            exists.IsLocked = user.IsLocked ?? exists.IsLocked;
        }
    }
}