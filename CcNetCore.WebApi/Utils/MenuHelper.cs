using System.Collections.Generic;
using System.Linq;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Helpers;
using Newtonsoft.Json;

namespace CcNetCore.WebApi.Utils {
    /// <summary>
    /// 菜单帮助类
    /// </summary>
    public static class MenuHelper {
        /// <summary>
        /// 加载菜单树列表
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="selectedGuid"></param>
        /// <returns></returns>
        public static List<MenuItem> LoadMenuItems (this List<MenuDto> menus, string selectedGuid = null) {
            var tree = menus.Select (x => new MenuItem {
                Guid = x.Uid.GetValue (),
                    ParentId = x.ParentUid.GetValue (),
                    Name = x.Alias.GetValue (),
                    Path = $"/{x.Url}",
                    Component = x.Component.GetValue (),
                    Meta = new MenuMeta {
                        BeforeCloseFun = x.BeforeCloseFun.GetValue (),
                            HideInMenu = x.HideInMenu ?? false,
                            Icon = x.Icon.GetValue (),
                            NotCache = x.NotCache ?? false,
                            Title = x.Name.GetValue ()
                    }
            });

            var result = tree.BuildTree (selectedGuid, (item) => new MenuItem () {
                Guid = item.Guid.GetValue (),
                    ParentId = item.ParentId.GetValue (),
                    Component = item.Component.GetValue ("Main"),
                    Name = item.Name.GetValue (),
                    Path = item.Path.GetValue (),
                    Meta = item.Meta
            });

            return result;
        }

        /// <summary>
        /// 加载菜单树列表
        /// </summary>
        /// <param name="selectedGuid"></param>
        /// <returns></returns>
        public static List<MenuTree> LoadMenuTree (this IEnumerable<MenuDto> menus, string selectedGuid = null) {
            if (menus.IsEmpty ()) {
                return null;
            }

            var tree = menus.Select (x => new MenuTree {
                Guid = x.Uid.GetValue (),
                    ParentGuid = x.ParentUid.GetValue (),
                    Title = x.Name.GetValue ()
            })?.ToList ();

            var root = new MenuTree {
                Title = "顶级菜单",
                Guid = Constants.UID_EMPTY,
                ParentGuid = string.Empty
            };

            tree.Insert (0, root);

            return tree.BuildTree (selectedGuid.GetValue (), (x) => new MenuTree () {
                Guid = x.Guid.GetValue (),
                    ParentGuid = x.ParentGuid.GetValue (),
                    Title = x.Title.GetValue (),
                    Expand = (!x.ParentGuid.IsValid () || Constants.UID_EMPTY == x.ParentGuid),
                    Selected = (selectedGuid == x.Guid),
            });
        }
    }

    /// <summary>
    /// 菜单项
    /// </summary>
    public class MenuItem : ITreeNode<string, MenuItem> {
        public MenuItem () {
            Meta = new MenuMeta ();
            Children = new List<MenuItem> ();
        }

        #region ITreeNode接口
        [JsonIgnore]
        public string Key => Guid;

        [JsonIgnore]
        public string ParentKey => ParentId;
        #endregion

        public string Guid { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Component { get; set; }
        public string ParentId { get; set; }
        public MenuMeta Meta { get; set; }
        public List<MenuItem> Children { get; set; }
    }

    public class MenuMeta {
        public MenuMeta () {
            Permission = new List<string> ();
            BeforeCloseFun = string.Empty;
        }

        public bool HideInMenu { get; set; }
        public string Icon { get; set; }
        public bool NotCache { get; set; }
        public string Title { get; set; }
        public List<string> Permission { get; set; }
        public string BeforeCloseFun { get; set; }
    }

    /// <summary>
    /// 用于iview的菜单树
    /// </summary>
    public class MenuTree : ITreeNode<string, MenuTree> {
        public MenuTree () {
            Children = new List<MenuTree> ();
        }

        #region ITreeNode接口
        [JsonIgnore]
        public string Key => Guid;

        [JsonIgnore]
        public string ParentKey => ParentGuid;
        #endregion

        public string Guid { get; set; }
        public string ParentGuid { get; set; }
        public string Title { get; set; }
        public bool Expand { get; set; }
        public bool Disabled { get; set; }
        public bool DisableCheckbox { get; set; }
        public bool Selected { get; set; }
        public bool Checked { get; set; }
        public List<MenuTree> Children { get; set; }
    }
}