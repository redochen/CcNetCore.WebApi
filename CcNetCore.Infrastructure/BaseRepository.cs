using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using Dapper.Contrib.Extensions;
using CcNetCore.Domain;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;
using CcNetCore.Domain.Repositories;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.Infrastructure {
    /// <summary>
    /// 数据仓储基类
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class BaseRepository<TDto, TEntity> : DapperRepository<TEntity>, IRepository<TDto>
        where TDto : BaseDto, new ()
    where TEntity : BaseEntity, new () {
        //自动装载属性（必须为public，否则自动装载失败）
        public IMapper _Mapper { get; set; }

        /// <summary>
        /// 查询所有项
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <returns></returns>
        public new (IEnumerable<TDto>, Exception) Query (int pageSize, int pageIndex) {
            var (items, ex) = base.Query (pageSize, pageIndex);
            return (items?.Select (x => GetDto (x)), ex);
        }

        /// <summary>
        /// 根据已有项查询所有匹配的项列表
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <returns></returns>
        public (IEnumerable<TDto>, Exception) Query (TDto query) {
            var (items, ex) = base.Query (GetEntity (query));
            return (items?.Select (x => GetDto (x)), ex);
        }

        /// <summary>
        /// 根据已有项查询所有匹配的项列表
        /// </summary>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="query">查询实体</param>
        /// <returns></returns>
        public (IEnumerable<TDto>, Exception) Query (int pageSize, int pageIndex, TDto query) {
            var (items, ex) = base.Query (pageSize, pageIndex, GetEntity (query));
            return (items?.Select (x => GetDto (x)), ex);
        }

        /// <summary>
        /// 批量添加项
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public Exception Add (IEnumerable<TDto> items) =>
            base.Add (items.Select (x => GetEntity (x)));

        /// <summary>
        /// 更新项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Exception Update (TDto item) => base.Update (GetEntity (item));

        /// <summary>
        /// 根据已有项删除所有匹配的项列表
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Exception Delete (TDto item) => base.Delete (GetEntity (item));

        /// <summary>
        /// 将Dto转换成实体
        /// </summary>
        /// <param name="dto"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected TEntity GetEntity (TDto dto) => _Mapper.Map<TEntity> (dto);

        /// <summary>
        /// 将实体转换成Dto
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        protected TDto GetDto (TEntity entity) => _Mapper.Map<TDto> (entity);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="entities">要添加的数据项列表</param>
        /// <returns></returns>
        protected override Exception InsertItems (IDbConnection conn, IDbTransaction trans, IEnumerable<TEntity> entities) {
            if (entities.IsEmpty ()) {
                return Exceptions.InvalidParam;
            }

            TEntity exists = null;
            Exception ex = null;

            entities.ForEach (x => {
                ex = QueryExists (conn, x, isCreation : true, out TEntity item);
                exists = item;
                return (ex != null || exists != null);
            });

            if (ex != null) {
                return ex;
            }

            if (exists != null) {
                return Exceptions.AlreadyExists;
            }

            return conn.Insert (entities, trans) > 0 ? null : Exceptions.Failure;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="entity">要更新的数据项</param>
        /// <returns></returns>
        protected override Exception UpdateItem (IDbConnection conn, IDbTransaction trans, TEntity entity) {
            var ex = QueryExists (conn, entity, isCreation : false, out TEntity exists);
            if (ex != null) {
                return ex;
            }

            if (null == exists) {
                return Exceptions.NotFound;
            }

            UpdateExists (exists, entity);

            //统一字段处理
            exists.Status = entity.Status ?? exists.Status;
            exists.IsDeleted = entity.IsDeleted ?? exists.IsDeleted;
            exists.UpdateUser = entity.UpdateUser;
            exists.UpdateTime = DateTime.Now;

            return conn.Update (exists, trans) ? null : Exceptions.Failure;
        }

        /// <summary>
        /// 查询已存在的数据项
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entity">要保存的数据项</param>
        /// <param name="isCreation">是否为创建，否则为更新</param>
        /// <param name="exists">已存在的数据项</param>
        /// <returns></returns>
        protected abstract Exception QueryExists (IDbConnection conn, TEntity entity, bool isCreation, out TEntity exists);

        /// <summary>
        /// 更新已存在的数据项
        /// </summary>
        /// <param name="exists">已存在的数据项</param>
        /// <param name="entity">要保存的数据项</param>
        protected abstract void UpdateExists (TEntity exists, TEntity entity);
    }
}