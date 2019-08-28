using System;
using System.Collections.Generic;
using System.Linq;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.WebApi.Utils {
    /// <summary>
    /// 用户可以访问的控制器及操作权限
    /// </summary>
    public class AccessPermission {
        /// <summary>
        /// 控制器
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Action集合
        /// </summary>
        public List<string> Actions { get; set; }
    }

    /// <summary>
    /// 用户拥有的API操作权限
    /// </summary>
    public class AccessPermissions {
        /// <summary>
        /// 可以访问的API控制器集合
        /// </summary>
        public List<AccessPermission> Permissions { get; private set; }

        /// <summary>
        /// 是否可以访问
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool CanAccess (string controller, string action) {
            if (!controller.IsValid () || !action.IsValid ()) {
                return false;
            }
            var ctrl = Permissions.Where (x => controller.EqualsEx (x.Controller, ignoreCase : true))
                .FirstOrDefault (x => x.Controller == controller);
            if (null == ctrl) {
                return false;
            }

            return ctrl.Actions.Contains (action, StringComparer.OrdinalIgnoreCase);
        }
    }
}