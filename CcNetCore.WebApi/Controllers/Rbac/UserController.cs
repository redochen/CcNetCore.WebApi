using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 用户管理接口
    /// </summary>
    [Route ("api/rbac/user")]
    [ApiController]
    public class UserController : SysController<UserDto>, IApiController {
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("add")]
        public Result Create ([FromBody] CreateUserDto dto) => base.Create (dto);

        /// <summary>
        /// 更新用户资料
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("update")]
        public Result Update ([FromBody] UpdateUserDto dto) => base.Update (dto);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="dto">模型</param>
        /// <returns></returns>
        [HttpPost ("delete")]
        public Result Delete ([FromBody] BatchDto dto) => base.Delete (dto);

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="pageSize">每页项数</param>
        /// <param name="pageNo">页码，从1开始</param>
        /// <param name="uid">惟一标识</param>
        /// <param name="status">状态</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="nickName"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        [HttpGet ("list")]
        public PageResult<UserDto> GetList (int pageSize = 0, int pageNo = 1,
            string uid = "", Status? status = null, int? userId = null,
            string userName = "", string nickName = "", UserType? userType = null) {
            var cond = new UserDto {
            UserID = userId??0,
            Uid = uid,
            Status = status,
            UserType = userType,
            UserName = userName,
            NickName = nickName,
            };

            return base.GetPagedList (cond, pageSize, pageNo);
        }
    }
}