using CcNetCore.Application.Models;
using CcNetCore.Common;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 角色管理接口
    /// </summary>
    [Route ("api/rbac/role")]
    [ApiController]
    public class RoleController : BaseController<RoleModel>, IApiController {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("add")]
        [HttpPost]
        public BaseResult Create ([FromBody] CreateRoleModel model) => base.Create (model);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("update")]
        [HttpPost]
        public BaseResult Update ([FromBody] UpdateRoleModel model) => base.Update (model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("delete")]
        [HttpPost]
        public BaseResult Delete ([FromBody] DeleteRoleModel model) => base.Delete (model);

        /// <summary>
        /// 查询角色列表
        /// </summary>
        /// <param name="pageSize">每页项数</param>
        /// <param name="pageNo">页码，从1开始</param>
        /// <param name="uid">惟一标识</param>
        /// <param name="status">状态</param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="isSuperAdmin"></param>
        /// <param name="isBuiltin"></param>
        /// <returns></returns>
        [Route ("get")]
        [HttpGet]
        public PageQueryResult<RoleModel> GetRoles (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, string code = "", string name = "",
            bool? isSuperAdmin = null, bool? isBuiltin = null) {
            var cond = new RoleModel {
            Uid = uid,
            Status = status,
            Code = code,
            Name = name,
            IsSuperAdmin = isSuperAdmin,
            IsBuiltin = isBuiltin
            };

            return base.GetPagedList (cond, pageSize, pageNo);
        }
    }
}