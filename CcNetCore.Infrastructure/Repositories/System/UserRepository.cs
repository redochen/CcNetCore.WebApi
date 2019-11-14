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
    /// 用户仓储类
    /// </summary>
    public class UserRepository : EntityRepository<User>, IUserRepository, ISingletonInstance {
        /// <summary>
        /// 查询已存在的数据项
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="user">要保存的数据项</param>
        /// <returns></returns>
        protected override (User, Exception) QueryExists (IDbConnection conn, User user) {
            if (!user.UserName.IsValid () || !user.PasswordHash.IsValid ()) {
                return (null, Exceptions.InvalidParam);
            }

            var matchFields = new string[] {
                nameof (user.Uid), nameof (user.UserName)
            };

            var (_, items, ex) = Select (user, matchFields);
            return (items?.FirstOrDefault (), ex);
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