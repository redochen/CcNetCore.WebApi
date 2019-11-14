using System;
using CcNetCore.Application;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 消息接口
    /// </summary>
    [Route ("api/message")]
    [ApiController]
    public class MessageController : BaseController, IApiController {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpGet ("count")]
        public IActionResult Count () {
            return Ok (1);
        }

        /// <summary>
        /// 初始化消息标题列表
        /// </summary>
        /// <returns></returns>
        [HttpGet ("init")]
        public IResult Init () => Result<object>.GetResult (new object[] {
            new { title = "消息1", create_time = DateTime.Now, msg_id = 1 }
        });

        /// <summary>
        /// 获取指定ID的消息内容
        /// </summary>
        /// <returns></returns>
        [HttpGet ("content/{msgid}")]
        public IResult Content ([FromRoute] int msgid) =>
            Result<string>.GetResult ($"消息[{msgid}]内容");

        /// <summary>
        /// 将消息标为已读
        /// </summary>
        /// <returns></returns>
        [HttpGet ("has_read/{msgid}")]
        public IResult HasRead ([FromRoute] int msgid) => ErrorCode.Success.ToResult ();

        /// <summary>
        /// 删除已读消息
        /// </summary>
        /// <returns></returns>
        [HttpGet ("remove_readed/{msgid}")]
        public IResult RemoveRead ([FromRoute] int msgid) => ErrorCode.Success.ToResult ();

        /// <summary>
        /// 恢复已删消息
        /// </summary>
        /// <returns></returns>
        [HttpGet ("restore/{msgid}")]
        public IResult Restore ([FromRoute] int msgid) => ErrorCode.Success.ToResult ();
    }
}