using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 角色权限管理接口
    /// </summary>
    [Route ("api/rbac/role_perm")]
    [ApiController]
    public class RolePermController : SysController<RolePermDto>, IApiController {
        //自动装载属性（必须为public，否则自动装载失败）
        public new IRolePermService _Service { get; set; }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("save")]
        public Result Save ([FromBody] SaveRolePermDto dto) =>
            HandleRequest<Result> ((userID) => _Service.Save (userID, dto));

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
        [HttpGet ("list")]
        public PageResult<RolePermDto> GetList (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, string roleCode = "", string permCode = "") {
            var cond = new RolePermDto {
            Uid = uid,
            Status = status,
            RoleCode = roleCode,
            PermCode = permCode,
            };

            return base.GetPagedList (cond, pageSize, pageNo);
        }
    }
}