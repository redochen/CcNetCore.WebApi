using Newtonsoft.Json;
using CcNetCore.Application.Interfaces;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 创建角色模型
    /// </summary>
    public class CreateRoleModel : ICreateModel {
        /// <summary>
        /// 角色名称
        /// </summary>
        [JsonRequired]
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否是超级管理员(超级管理员拥有系统的所有权限)
        /// </summary>
        public bool? IsSuperAdmin { get; set; }
    }
}