using Newtonsoft.Json;
using CcNetCore.Application.Interfaces;
using CcNetCore.Common;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 修改用户模块
    /// </summary>
    public class UpdateUserModel : IUpdateModel {
        /// <summary>
        /// 惟一标识
        /// </summary>
        /// <value></value>
        [JsonRequired]
        public string Uid { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 显示昵名
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 显示头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType? UserType { get; set; }

        /// <summary>
        /// 用户描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Status? Status { get; set; }
    }
}