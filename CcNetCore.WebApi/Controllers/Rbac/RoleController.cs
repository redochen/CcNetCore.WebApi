using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 角色管理接口
    /// </summary>
    [Route ("api/rbac/role")]
    [ApiController]
    public class RoleController : SysController<RoleDto>, IApiController {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("add")]
        public Result Create ([FromBody] CreateRoleDto dto) => base.Create (dto);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("update")]
        public Result Update ([FromBody] UpdateRoleDto dto) => base.Update (dto);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("delete")]
        public Result Delete ([FromBody] BatchDto dto) => base.Delete (dto);

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
        [HttpGet ("list")]
        public PageResult<RoleDto> GetList (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, string code = "", string name = "",
            bool? isSuperAdmin = null, bool? isBuiltin = null) {
            var cond = new RoleDto {
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