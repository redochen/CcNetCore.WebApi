using Newtonsoft.Json;
using CcNetCore.Application.Interfaces;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 删除角色权限模型
    /// </summary>
    public class DeleteRolePermModel : IDeleteModel {
        /// <summary>
        /// 惟一标识集合
        /// </summary>
        /// <value></value>
        [JsonRequired]
        public string[] Uid { get; set; }
    }
}