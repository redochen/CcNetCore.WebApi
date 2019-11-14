using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 权限管理接口
    /// </summary>
    [Route ("api/rbac/perm")]
    [ApiController]
    public class PermController : SysController<PermDto>, IApiController {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("add")]
        public Result Create ([FromBody] CreatePermDto dto) => base.Create (dto);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("update")]
        public Result Update ([FromBody] UpdatePermDto dto) => base.Update (dto);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("delete")]
        public Result Delete ([FromBody] BatchDto dto) => base.Delete (dto);

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
        [HttpGet ("list")]
        public PageResult<PermDto> GetList (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, string permCode = "", string permName = "",
            PermType? permType = null, string menuGuid = "", string actionCode = "") {
            var cond = new PermDto {
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