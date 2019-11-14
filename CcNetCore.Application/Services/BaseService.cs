using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 服务基类
    /// </summary>
    public class BaseService {
        //自动装载属性（必须为public，否则自动装载失败）
        public IMapper _Mapper { get; set; }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="dto"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected TEntity GetEntity<TEntity> (object dto) where TEntity : BaseEntity, new () =>
            _Mapper.Map<TEntity> (dto);

        /// <summary>
        /// 获取DTO
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        protected TDto GetDto<TDto> (object entity) where TDto : BaseDto, new () =>
            _Mapper.Map<TDto> (entity);
    }

    /// <summary>
    /// 服务基类
    /// </summary>
    public class BaseService<TDto, TEntity> : BaseService, IService<TDto>
        where TDto : BaseDto, new ()
    where TEntity : BaseEntity, new () {
        //自动装载属性（必须为public，否则自动装载失败）
        public virtual IRepository<TEntity> _Repo { get; set; }

        /// <summary>
        /// 根据uid查询
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public virtual Result<TDto> Get (string uid) {
            if (!uid.IsValid ()) {
                return Exceptions.InvalidParam.ToResult<Result<TDto>> ();
            }

            var queryResult = Get (new TDto {
                Uid = uid,
            });

            var result = queryResult.ToResult<Result<TDto>> ();
            result.Data = queryResult.Items?.First ();

            return result;
        }

        /// <summary>
        /// 查询列表（无分页）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public virtual ListResult<TDto> Get (TDto dto) {
            if (null == dto) {
                return Exceptions.InvalidParam.ToResult<ListResult<TDto>> ();
            }

            var queryResult = Get (new Query<TDto> {
                Condition = dto
            });

            var result = queryResult.ToResult<ListResult<TDto>> ();
            result.Items = queryResult.Items;

            return result;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public virtual PageResult<TDto> Get (Query<TDto> query) {
            var cond = new Query<TEntity> (query);
            cond.Condition = GetEntity<TEntity> (query?.Condition);

            if (null == cond.Condition) {
                cond.Condition = new TEntity ();
            }

            cond.PageNo = Math.Max (1, cond.PageNo);

            //查询未删除的数据
            cond.Condition.IsDeleted = 0;

            var (count, items, ex) = _Repo.Select (cond.PageSize, cond.PageNo - 1, cond.Condition);

            var result = new PageResult<TDto> (cond);
            result.SetError (ex);

            result.TotalCount = count;
            result.Items = items?.Select (x => GetDto<TDto> (x))?.ToList ();

            if (cond.PageSize > 0) {
                result.TotalPages = (long) Math.Ceiling (count * 1.0D / cond.PageSize);
            } else {
                result.TotalPages = count > 0 ? 1 : 0;
            }

            return result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public virtual Result Create (int userID, ICreateDto dto) {
            var entities = GetCreateEntities (dto, (entity) => HandleCreateEntity (userID, entity));
            if (entities.IsEmpty ()) {
                return Exceptions.InvalidParam.ToResult ();
            }

            return _Repo.Add (entities).ToResult ();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public virtual Result Update (int userID, IUpdateDto dto) {
            var entity = GetEntity<TEntity> (dto);
            if (null == entity) {
                return Exceptions.InvalidParam.ToResult ();
            }

            //默认为未删除
            entity.IsDeleted = 0;
            entity.UpdateUser = userID;
            entity.UpdateTime = DateTime.Now;

            return _Repo.Update (entity).ToResult ();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public virtual Result Delete (int userID, IDeleteDto dto) {
            var entity = GetEntity<TEntity> (dto);
            if (null == entity) {
                return Exceptions.InvalidParam.ToResult ();
            }

            //使用软删除
            entity.IsDeleted = 1;
            entity.UpdateUser = userID;
            entity.UpdateTime = DateTime.Now;

            return _Repo.Update (entity).ToResult ();
            //return _Repo.Delete (entity).ToResult ();
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Result BatchDelete (int userID, IBatchDto dto) {
            if (null == dto || dto.Uid.IsEmpty ()) {
                return Exceptions.InvalidParam.ToResult ();
            }

            var entity = new TEntity {
                IsDeleted = 1,
            };

            return BatchUpdate (userID, dto.Uid, entity, nameof (entity.IsDeleted));
        }
        /// <summary>
        /// 批量恢复
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Result BatchRecover (int userID, IBatchDto dto) {
            if (null == dto || dto.Uid.IsEmpty ()) {
                return Exceptions.InvalidParam.ToResult ();
            }

            var entity = new TEntity {
                IsDeleted = 0,
            };

            return BatchUpdate (userID, dto.Uid, entity, nameof (entity.IsDeleted));
        }

        /// <summary>
        /// 批量修改状态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="dto"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Result BatchUpdateStatus (int userID, IBatchDto dto, int status) {
            if (null == dto || dto.Uid.IsEmpty ()) {
                return Exceptions.InvalidParam.ToResult ();
            }

            var entity = new TEntity {
                Status = status
            };

            return BatchUpdate (userID, dto.Uid, entity, nameof (entity.Status));
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="uids"></param>
        /// <param name="entity">要更新的值</param>
        /// <returns></returns>
        protected Result BatchUpdate (int userID, IEnumerable<string> uids,
            TEntity entity, params string[] updateFields) {
            entity.UpdateUser = userID;
            entity.UpdateTime = DateTime.Now;

            updateFields.Append (nameof (entity.UpdateUser));
            updateFields.Append (nameof (entity.UpdateTime));

            return _Repo.UpdateIn ("Uid", uids, entity, updateFields).ToResult ();
        }

        /// <summary>
        /// 获取创建实体
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="handleCreate"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TEntity> GetCreateEntities (
            ICreateDto dto, Action<TEntity> handleCreate) {
            var entity = GetEntity<TEntity> (dto);
            handleCreate?.Invoke (entity);
            return new List<TEntity> { entity };
        }

        /// <summary>
        /// 处理创建实体
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="entity"></param>
        protected virtual void HandleCreateEntity (int userID, TEntity entity) {
            entity.Uid = Guid.NewGuid ().ToString ("N");
            entity.IsDeleted = 0; //默认为未删除
            entity.CreateUser = userID;
            entity.CreateTime = DateTime.Now;
        }
    }

    /// <summary>
    /// 系统服务基类
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class SysService<TDto, TEntity> : BaseService<TDto, TEntity>, ISysService<TDto>
        where TDto : SysDto, new ()
    where TEntity : BaseEntity, new () {
        /// <summary>
        /// 批量修改状态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="dto"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Result BatchUpdateStatus (int userID, IBatchDto dto, Status status) =>
            base.BatchUpdateStatus (userID, dto, (int) status);

        /// <summary>
        /// 处理创建实体
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="entity"></param>
        protected override void HandleCreateEntity (int userID, TEntity entity) {
            base.HandleCreateEntity (userID, entity);

            //默认为正常状态
            entity.Status = entity.Status ?? (int) Status.Normal;
        }
    }

    /// <summary>
    /// 业务服务基类
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class BizService<TDto, TEntity> : BaseService<TDto, TEntity>, IBizService<TDto>
        where TDto : BizDto, new ()
    where TEntity : BaseEntity, new () {
        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Result BatchAudit (int userID, IBatchDto dto) =>
            base.BatchUpdateStatus (userID, dto, (int) AuditStatus.Audited);

        /// <summary>
        /// 处理创建实体
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="entity"></param>
        protected override void HandleCreateEntity (int userID, TEntity entity) {
            base.HandleCreateEntity (userID, entity);

            //默认为未审核状态
            entity.Status = entity.Status ?? (int) AuditStatus.UnAudited;
        }
    }
}