using CcNetCore.Application.Models;
using CcNetCore.Common;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 菜单管理接口
    /// </summary>
    [Route ("api/rbac/menu")]
    [ApiController]
    public class MenuController : BaseController<MenuModel>, IApiController {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("add")]
        [HttpPost]
        public BaseResult Create ([FromBody] CreateMenuModel model) => base.Create (model);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("update")]
        [HttpPost]
        public BaseResult Update ([FromBody] UpdateMenuModel model) => base.Update (model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        [Route ("delete")]
        [HttpPost]
        public BaseResult Delete ([FromBody] DeleteMenuModel model) => base.Delete (model);

        /// <summary>
        /// 查询菜单列表
        /// </summary>
        /// <param name="pageSize">每页项数</param>
        /// <param name="pageNo">页码，从1开始</param>
        /// <param name="uid">惟一标识</param>
        /// <param name="status">状态</param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="alias"></param>
        /// <param name="ParentUid"></param>
        /// <returns></returns>
        [Route ("get")]
        [HttpGet]
        public PageQueryResult<MenuModel> GetMenus (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, string name = "", string url = "",
            string alias = "", string ParentUid = "") {
            var cond = new MenuModel {
            Uid = uid,
            Status = status,
            Name = name,
            Alias = alias,
            Url = url,
            ParentUid = ParentUid,
            };

            return base.GetPagedList (cond, pageSize, pageNo);
        }
    }
}