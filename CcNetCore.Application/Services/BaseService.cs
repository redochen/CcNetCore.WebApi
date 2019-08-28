using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 服务基类
    /// </summary>
    public class BaseService<TModel, TDto> : IService<TModel>
        where TModel : BaseModel, new ()
    where TDto : BaseDto, new () {
        //自动装载属性（必须为public，否则自动装载失败）
        public IMapper _Mapper { get; set; }

        //自动装载属性（必须为public，否则自动装载失败）
        public virtual IRepository<TDto> _Repo { get; set; }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual BaseResult Create (int userID, ICreateModel model) {
            var dtos = GetCreateDtos (model, (dto) => HandleCreateDto (userID, dto));
            if (dtos.IsEmpty ()) {
                return Exceptions.InvalidParam.ToResult ();
            }

            return _Repo.Add (dtos).ToResult ();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual BaseResult Update (int userID, IUpdateModel model) {
            var dto = GetDto (model);
            if (null == dto) {
                return Exceptions.InvalidParam.ToResult ();
            }

            dto.UpdateUser = userID;
            dto.UpdateTime = DateTime.Now;

            return _Repo.Update (dto).ToResult ();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual BaseResult Delete (int userID, IDeleteModel model) {
            var dto = GetDto (model);
            if (null == dto) {
                return Exceptions.InvalidParam.ToResult ();
            }

            //使用软删除
            dto.IsDeleted = 1;
            dto.UpdateUser = userID;
            dto.UpdateTime = DateTime.Now;

            return _Repo.Update (dto).ToResult ();
            //return _Repo.Delete (dto).ToResult ();
        }

        /// <summary>
        /// 删除所有匹配的项列表
        /// </summary>
        /// <param name="inField">批量删除时IN匹配的字段名</param>
        /// <param name="inValues"></param>
        /// <returns></returns>
        protected Exception BatchDelete (string inField, IEnumerable<object> inValues) =>
            _Repo.Delete (inField, inValues);

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual PageQueryResult<TModel> Query (PageQueryModel<TModel> model) {
            var query = PageQueryModel<TDto>.Parse (model);
            query.Condition = GetDto (model?.Condition);
            if (null == query.Condition) {
                query.Condition = new TDto ();
            }

            //查询未删除的数据
            query.Condition.IsDeleted = 0;

            var (items, ex) = _Repo.Query (query.PageSize, query.PageIndex, query.Condition);

            var result = ex.ToResult<PageQueryResult<TModel>> ();
            result.Items = items?.Select (x => GetModel (x))?.ToList ();

            return result;
        }

        /// <summary>
        /// 获取创建DTO
        /// </summary>
        /// <param name="model"></param>
        /// <param name="handleDto"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TDto> GetCreateDtos (ICreateModel model, Action<TDto> handleDto) {
            var dto = GetDto (model);
            handleDto?.Invoke (dto);
            return new List<TDto> { dto };
        }

        /// <summary>
        /// 处理创建Dto
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="dto"></param>
        protected virtual void HandleCreateDto (int userID, TDto dto) {
            dto.Uid = Guid.NewGuid ().ToString ("N");
            dto.Status = dto.Status ?? Common.Status.Normal; //默认为正常状态
            dto.IsDeleted = 0; //默认为未删除
            dto.CreateUser = userID;
            dto.CreateTime = DateTime.Now;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected TDto GetDto (object model) => _Mapper.Map<TDto> (model);

        /// <summary>
        ///
        /// </summary>
        /// <param name="dto"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        protected TModel GetModel (TDto dto) => _Mapper.Map<TModel> (dto);
    }
}