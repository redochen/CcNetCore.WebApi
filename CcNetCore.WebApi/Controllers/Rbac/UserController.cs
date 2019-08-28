using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 用户管理接口
    /// </summary>
    [Route ("api/rbac/user")]
    [ApiController]
    public class UserController : BaseController<UserModel>, IApiController {
        //自动装载属性（必须为public，否则自动装载失败）
        public new IUserService _Service { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("add")]
        [HttpPost]
        public BaseResult Create ([FromBody] CreateUserModel model) => base.Create (model);

        /// <summary>
        /// 更新用户资料
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("update")]
        [HttpPost]
        public BaseResult Update ([FromBody] UpdateUserModel model) => base.Update (model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("delete")]
        [HttpPost]
        public BaseResult Delete ([FromBody] DeleteUserModel model) => base.Delete (model);

        /// <summary>
        /// 修改登录密码
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("change_pwd")]
        [HttpPost]
        public BaseResult ChangePwd ([FromBody] ChangePwdModel model) =>
            HandleRequest<BaseResult> ((userID) => {
                var result = _Service.ChangePwd (model);
                if (result.Exception is IdentityException) {
                    result.Message = "密码错误";
                }

                return result;
            });

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="pageSize">每页项数</param>
        /// <param name="pageNo">页码，从1开始</param>
        /// <param name="uid">惟一标识</param>
        /// <param name="status">状态</param>
        /// <param name="userName"></param>
        /// <param name="nickName"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        [Route ("get")]
        [HttpGet]
        public PageQueryResult<UserModel> GetUsers (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, string userName = "",
            string nickName = "", UserType? userType = null) {
            var cond = new UserModel {
            Uid = uid,
            Status = status,
            UserType = userType,
            UserName = userName,
            NickName = nickName,
            };

            return base.GetPagedList (cond, pageSize, pageNo);
        }
    }
}