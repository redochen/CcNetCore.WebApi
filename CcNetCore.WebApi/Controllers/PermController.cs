using Microsoft.AspNetCore.Mvc;
using CcNetCore.Application.Models;
using CcNetCore.Common;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 权限管理接口
    /// </summary>
    [Route ("api/v1/perm")]
    [ApiController]
    public class PermController : BaseController<PermModel>, IApiController {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("add")]
        [HttpPost]
        public BaseResult Create ([FromBody] CreatePermModel model) => base.Create (model);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("update")]
        [HttpPost]
        public BaseResult Update ([FromBody] UpdatePermModel model) => base.Update (model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("delete")]
        [HttpPost]
        public BaseResult Delete ([FromBody] DeletePermModel model) => base.Delete (model);

        /// <summary>
        /// 查询权限列表
        /// </summary>
        /// <param name="pageSize">每页项数</param>
        /// <param name="pageNo">页码，从1开始</param>
        /// <param name="uid">惟一标识</param>
        /// <param name="status">状态</param>
        /// <param name="permCode"></param>
        /// <param name="permName"></param>
        /// <param name="permType"></param>
        /// <param name="menuGuid"></param>
        /// <param name="actionCode"></param>
        /// <returns></returns>
        [Route ("getPerms")]
        [HttpGet]
        public PageQueryResult<PermModel> GetPermissions (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, string permCode = "", string permName = "",
            PermType? permType = null, string menuGuid = "", string actionCode = "") {
            var cond = new PermModel {
            Uid = uid,
            Status = status,
            Code = permCode,
            Name = permName,
            Type = permType,
            MenuGuid = menuGuid,
            ActionCode = actionCode
            };

            return base.GetPagedList (cond, pageSize, pageNo);
        }
    }
}