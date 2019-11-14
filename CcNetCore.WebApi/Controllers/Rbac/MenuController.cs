using System;
using System.Collections.Generic;
using CcNetCore.Application;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 菜单管理接口
    /// </summary>
    [Route ("api/rbac/menu")]
    [ApiController]
    public class MenuController : SysController<MenuDto>, IApiController {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("add")]
        public Result Create ([FromBody] CreateMenuDto dto) => base.Create (dto);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("update")]
        public Result Update ([FromBody] UpdateMenuDto dto) => base.Update (dto);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("delete")]
        public Result Delete ([FromBody] BatchDto dto) => base.Delete (dto);

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost ("recover")]
        public Result Recover ([FromBody] BatchDto dto) => base.Recover (dto);

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost ("batch/{cmd}")]
        public IResult Batch ([FromRoute] string cmd, [FromBody] BatchDto dto) {
            BatchOperation opCode;
            if (!Enum.TryParse (cmd, ignoreCase : true, out opCode)) {
                return ErrorCode.UnSupported.ToResult ();
            }

            return base.Batch (opCode, dto);
        }

        /// <summary>
        /// 根据UID获取菜单
        /// </summary>
        /// <param name="uid">惟一标识</param>
        /// <returns></returns>
        [HttpGet ("get/{uid}")]
        public Result<MenuDto> Get ([FromRoute] string uid) => base.GetByUid (uid);

        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <returns></returns>
        [HttpGet ("tree")]
        public IResult Tree (string selected = null) =>
            Result<List<MenuTree>>.GetResult (LoadMenuTree (selected));

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
        [HttpGet ("list")]
        public PageResult<MenuDto> GetList (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, string name = "", string url = "",
            string alias = "", string ParentUid = "") {
            var cond = new MenuDto {
            Uid = uid,
            Status = status,
            Name = name,
            Alias = alias,
            Url = url,
            ParentUid = ParentUid,
            };

            return base.GetPagedList (cond, pageSize, pageNo);
        }

        /// <summary>
        /// 加载菜单树
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private List<MenuTree> LoadMenuTree (string guid) {
            var result = _Service.Get (new MenuDto { Status = Status.Normal });
            if (!result.IsSuccess ()) {
                return null;
            }

            return result.Items.LoadMenuTree (guid);
        }
    }
}