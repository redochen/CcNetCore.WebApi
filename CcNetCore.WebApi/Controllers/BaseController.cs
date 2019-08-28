using System;
using AutoMapper;
using CcNetCore.Application;
using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CcNetCore.WebApi.Controllers {
    /// <summary>
    /// 控制器接口
    /// </summary>
    public interface IApiController { }

    /// <summary>
    /// API接口基类
    /// </summary>
    [ApiController]
    public class BaseController<TModel> : ControllerBase where TModel : BaseModel, new () {
        //自动装载属性（必须为public，否则自动装载失败）
        public IMapper _Mapper { get; set; }

        //自动装载属性（必须为public，否则自动装载失败）
        public IService<TModel> _Service { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected BaseResult Create (ICreateModel model) =>
            HandleRequest<BaseResult> ((userID) => _Service.Create (userID, model));

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected BaseResult Update (IUpdateModel model) =>
            HandleRequest<BaseResult> ((userID) => _Service.Update (userID, model));

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected BaseResult Delete (IDeleteModel model) =>
            HandleRequest<BaseResult> ((userID) => _Service.Delete (userID, model));

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected BaseResult Delete (IBatchDeleteModel model) =>
            HandleRequest<BaseResult> ((userID) => _Service.BatchDelete (userID, model));

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

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageSize">每页项数</param>
        /// <param name="pageNo">页码，从1开始</param>
        /// <returns></returns>
        protected PageQueryResult<TModel> GetPagedList (TModel condition, int pageSize = 0, int pageNo = 1) =>
            HandleRequest<PageQueryResult<TModel>> ((userID) => {
                var model = new PageQueryModel<TModel> {
                Condition = condition,
                PageSize = pageSize,
                PageIndex = pageNo - 1,
                };

                return _Service.Query (model);
            });
    }
}