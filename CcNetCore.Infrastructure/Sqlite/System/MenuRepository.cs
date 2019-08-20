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
    /// 菜单仓储类
    /// </summary>
    public class MenuRepository : SqliteRepository<MenuDto, Menu>, IMenuRepository, ISingletonInstance {
        /// <summary>
        /// 查询已存在的数据项
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="menu">要保存的数据项</param>
        /// <param name="isCreation">是否为创建，否则为更新</param>
        /// <param name="exists">已存在的数据项</param>
        /// <returns></returns>
        protected override Exception QueryExists (IDbConnection conn, Menu menu, bool isCreation, out Menu exists) {
            exists = null;

            if (null == menu || !menu.Uid.IsValid ()) {
                return Exceptions.InvalidParam;
            }

            string[] matchFields = null;

            if (isCreation) {
                if (!menu.Name.IsValid () || !menu.Url.IsValid ()) {
                    return Exceptions.InvalidParam;
                }

                matchFields = new string[] {
                    nameof (menu.Uid), nameof (menu.Name), nameof (menu.Url)
                };
            } else {
                matchFields = new string[] {
                    nameof (menu.Uid), nameof (menu.IsDeleted)
                };
            }

            exists = conn.GetWhere (menu, MatchSql.OR, matchFields)?.FirstOrDefault ();

            return null;
        }

        /// <summary>
        /// 更新已存在的数据项
        /// </summary>
        /// <param name="exists">已存在的数据项</param>
        /// <param name="menu">要保存的数据项</param>
        protected override void UpdateExists (Menu exists, Menu menu) {
            exists.Name = menu.Name.GetValue (exists.Name);
            exists.Url = menu.Url.GetValue (exists.Url);
            exists.Alias = menu.Alias.GetValue (exists.Alias);
            exists.Icon = menu.Icon.GetValue (exists.Icon);
            exists.ParentUid = menu.ParentUid.GetValue (exists.ParentUid);
            exists.Description = menu.Description.GetValue (exists.Description);
            exists.Component = menu.Component.GetValue (exists.Component);
            exists.BeforeCloseFun = menu.BeforeCloseFun.GetValue (exists.BeforeCloseFun);
            exists.Level = menu.Level ?? exists.Level;
            exists.Sort = menu.Sort ?? exists.Sort;
            exists.IsDefault = menu.IsDefault ?? exists.IsDefault;
            exists.HideInMenu = menu.HideInMenu ?? exists.HideInMenu;
            exists.NotCache = menu.NotCache ?? exists.NotCache;
        }
    }
}