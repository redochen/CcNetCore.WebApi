using CcNetCore.Common;

namespace CcNetCore.Application.Models {
    /// <summary>
    /// 用户模型类
    /// </summary>
    public class UserModel : BaseModel {
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
    }
}