using Newtonsoft.Json;
using CcNetCore.Application.Interfaces;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 删除权限模型
    /// </summary>
    public class DeletePermModel : IDeleteModel {
        /// <summary>
        /// 惟一标识集合
        /// </summary>
        /// <value></value>
        [JsonRequired]
        public string[] Uid { get; set; }
    }
}