using Microsoft.AspNetCore.Mvc;
using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 角色权限管理接口
    /// </summary>
    [Route ("api/v1/rolePerm")]
    [ApiController]
    public class RolePermController : BaseController<RolePermModel>, IApiController {
        //自动装载属性（必须为public，否则自动装载失败）
        public new IRolePermService _Service { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("add")]
        [HttpPost]
        public BaseResult Create ([FromBody] CreateRolePermModel model) => base.Create (model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("delete")]
        [HttpPost]
        public BaseResult Remove ([FromBody] DeleteRolePermModel model) => base.Delete (model);

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("save")]
        [HttpPost]
        public BaseResult Save ([FromBody] SaveRolePermModel model) =>
            HandleRequest<BaseResult> ((userID) => _Service.Save (userID, model));

        /// <summary>
        /// 查询角色权限列表
        /// </summary>
        /// <param name="pageSize">每页项数</param>
        /// <param name="pageNo">页码，从1开始</param>
        /// <param name="uid">惟一标识</param>
        /// <param name="status">状态</param>
        /// <param name="roleCode"></param>
        /// <param name="permCode"></param>
        /// <returns></returns>
        [Route ("getRolePerms")]
        [HttpGet]
        public PageQueryResult<RolePermModel> GetRolePermissions (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, string roleCode = "", string permCode = "") {
            var cond = new RolePermModel {
            Uid = uid,
            Status = status,
            RoleCode = roleCode,
            PermCode = permCode,
            };

            return base.GetPagedList (cond, pageSize, pageNo);
        }
    }
}