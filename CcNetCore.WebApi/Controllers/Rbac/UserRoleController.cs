using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 用户角色管理接口
    /// </summary>
    [Route ("api/rbac/user_role")]
    [ApiController]
    public class UserRoleController : BaseController<UserRoleModel>, IApiController {
        //自动装载属性（必须为public，否则自动装载失败）
        public new IUserRoleService _Service { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("add")]
        [HttpPost]
        public BaseResult Create ([FromBody] CreateUserRoleModel model) => base.Create (model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("delete")]
        [HttpPost]
        public BaseResult Remove ([FromBody] DeleteUserRoleModel model) => base.Delete (model);

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("save")]
        [HttpPost]
        public BaseResult Save ([FromBody] SaveUserRoleModel model) =>
            HandleRequest<BaseResult> ((userID) => _Service.Save (userID, model));

        /// <summary>
        /// 查询用户角色列表
        /// </summary>
        /// <param name="pageSize">每页项数</param>
        /// <param name="pageNo">页码，从1开始</param>
        /// <param name="uid">惟一标识</param>
        /// <param name="status">状态</param>
        /// <param name="userUid"></param>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        [Route ("get")]
        [HttpGet]
        public PageQueryResult<UserRoleModel> GetUserRoles (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, string userUid = "", string roleCode = "") {
            var cond = new UserRoleModel {
            Uid = uid,
            Status = status,
            UserGuid = userUid,
            RoleCode = roleCode,
            };

            return base.GetPagedList (cond, pageSize, pageNo);
        }
    }
}