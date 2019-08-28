using CcNetCore.Application.Interfaces;
using Newtonsoft.Json;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 删除用户模型
    /// </summary>
    public class DeleteUserModel : IBatchDeleteModel {
        /// <summary>
        /// 惟一标识集合
        /// </summary>
        /// <value></value>
        [JsonRequired]
        public string[] Uid { get; set; }
    }
}