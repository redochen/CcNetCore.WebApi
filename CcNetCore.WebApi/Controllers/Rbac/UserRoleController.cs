using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 用户角色管理接口
    /// </summary>
    [Route ("api/rbac/user_role")]
    [ApiController]
    public class UserRoleController : SysController<UserRoleDto>, IApiController {
        //自动装载属性（必须为public，否则自动装载失败）
        public new IUserRoleService _Service { get; set; }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("save")]
        public Result Save ([FromBody] SaveUserRoleDto dto) =>
            HandleRequest<Result> ((userID) => _Service.Save (userID, dto));

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
        [HttpGet ("list")]
        public PageResult<UserRoleDto> GetList (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, string userUid = "", string roleCode = "") {
            var cond = new UserRoleDto {
            Uid = uid,
            Status = status,
            UserGuid = userUid,
            RoleCode = roleCode,
            };

            return base.GetPagedList (cond, pageSize, pageNo);
        }
    }
}