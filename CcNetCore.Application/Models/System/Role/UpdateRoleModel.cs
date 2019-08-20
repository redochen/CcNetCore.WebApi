using Newtonsoft.Json;
using CcNetCore.Application.Interfaces;
using CcNetCore.Common;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 更新角色模型
    /// </summary>
    public class UpdateRoleModel : IUpdateModel {
        /// <summary>
        /// 惟一标识
        /// </summary>
        /// <value></value>
        [JsonRequired]
        public string Uid { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否是超级管理员(超级管理员拥有系统的所有权限)
        /// </summary>
        public bool? IsSuperAdmin { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Status? Status { get; set; }
    }
}