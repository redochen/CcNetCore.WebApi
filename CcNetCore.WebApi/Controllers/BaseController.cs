using System;
using AutoMapper;
using CcNetCore.Application;
using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 控制器接口
    /// </summary>
    public interface IApiController { }

    public enum BatchOperation {
        delete,
        recover,
        forbidden,
        normal,
        audit,
    }

    /// <summary>
    /// API接口基类
    /// </summary>
    [ApiController]
    public class BaseController : ControllerBase {
        /// <summary>
        /// 处理用户请求
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        protected TResult HandleRequest<TResult> (Func<int /*userID*/ , TResult> handle)
        where TResult : IResult, new () {
            var userID = AuthContextService.CurrentUser?.UserID ?? 0;
            if (userID <= 0) {
                return ErrorCode.UnAuthorised.ToResult<TResult> ();
            }
            return handle (userID);
        }
    }

    /// <summary>
    /// API接口基类
    /// </summary>
    [ApiController]
    public class BaseController<TDto> : BaseController where TDto : BaseDto, new () {
        //自动装载属性（必须为public，否则自动装载失败）
        public IMapper _Mapper { get; set; }

        //自动装载属性（必须为public，否则自动装载失败）
        public IService<TDto> _Service { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected Result Create (ICreateDto dto) =>
            HandleRequest<Result> ((userID) => _Service.Create (userID, dto));

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected Result Update (IUpdateDto dto) =>
            HandleRequest<Result> ((userID) => _Service.Update (userID, dto));

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected Result Delete (IDeleteDto dto) =>
            HandleRequest<Result> ((userID) => _Service.Delete (userID, dto));

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected Result Delete (IBatchDto dto) =>
            HandleRequest<Result> ((userID) => _Service.BatchDelete (userID, dto));

        /// <summary>
        /// 批量恢复
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected Result Recover (IBatchDto dto) =>
            HandleRequest<Result> ((userID) => _Service.BatchRecover (userID, dto));

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="opCode">命令</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected Result Batch (BatchOperation opCode, IBatchDto dto) =>
            HandleRequest<Result> ((userID) => OnBatch (userID, opCode, dto));

        /// <summary>
        /// 执行批量操作
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="opCode"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected virtual Result OnBatch (int userID, BatchOperation opCode, IBatchDto dto) {
            switch (opCode) {
                case BatchOperation.delete:
                    return _Service.BatchDelete (userID, dto);
                case BatchOperation.recover:
                    return _Service.BatchRecover (userID, dto);
                default:
                    return Results.InvalidParam;
            }
        }

        /// <summary>
        /// 根据UID获取项
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        protected Result<TDto> GetByUid (string uid) =>
            HandleRequest<Result<TDto>> ((userID) => _Service.Get (uid));

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageSize">每页项数</param>
        /// <param name="pageNo">页码，从1开始</param>
        /// <returns></returns>
        protected PageResult<TDto> GetPagedList (TDto condition, int pageSize = 0, int pageNo = 1) =>
            HandleRequest<PageResult<TDto>> ((userID) => {
                var query = new Query<TDto> {
                Condition = condition,
                PageSize = pageSize,
                PageNo = pageNo,
                };

                return _Service.Get (query);
            });
    }

    /// <summary>
    /// 系统API接口基类
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    public class SysController<TDto> : BaseController<TDto> where TDto : SysDto, new () {
        //自动装载属性（必须为public，否则自动装载失败）
        public new ISysService<TDto> _Service { get; set; }

        /// <summary>
        /// 执行批量操作
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="opCode"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected override Result OnBatch (int userID, BatchOperation opCode, IBatchDto dto) {
            switch (opCode) {
                case BatchOperation.forbidden:
                    return _Service.BatchUpdateStatus (userID, dto, Status.Forbidden);
                case BatchOperation.normal:
                    return _Service.BatchUpdateStatus (userID, dto, Status.Normal);
                default:
                    return base.OnBatch (userID, opCode, dto);
            }
        }
    }

    /// <summary>
    /// 业务API接口基类
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    public class BizController<TDto> : BaseController<TDto> where TDto : BizDto, new () {
        //自动装载属性（必须为public，否则自动装载失败）
        public new IBizService<TDto> _Service { get; set; }

        /// <summary>
        /// 执行批量操作
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="opCode"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected override Result OnBatch (int userID, BatchOperation opCode, IBatchDto dto) {
            switch (opCode) {
                case BatchOperation.audit:
                    return _Service.BatchAudit (userID, dto);
                default:
                    return base.OnBatch (userID, opCode, dto);
            }
        }

        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected Result Audit (IBatchDto dto) =>
            HandleRequest<Result> ((userID) => _Service.BatchAudit (userID, dto));
    }
}