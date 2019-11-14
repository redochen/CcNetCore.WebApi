using CcNetCore.Application.Models;
using CcNetCore.Common;
using CcNetCore.Domain.Dtos;

namespace CcNetCore.Application.Interfaces {
    /// <summary>
    /// 服务接口
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    public interface IService<TDto> where TDto : BaseDto, new () {
        /// <summary>
        /// 根据uid查询
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        Result<TDto> Get (string uid);

        /// <summary>
        /// 查询列表（无分页）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        ListResult<TDto> Get (TDto dto);

        /// <summary>
        /// 查询列表（可分页）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        PageResult<TDto> Get (Query<TDto> query);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Result Create (int userID, ICreateDto dto);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Result Update (int userID, IUpdateDto dto);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Result Delete (int userID, IDeleteDto dto);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Result BatchDelete (int userID, IBatchDto dto);

        /// <summary>
        /// 批量恢复
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Result BatchRecover (int userID, IBatchDto dto);
    }

    /// <summary>
    /// 系统服务接口
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    public interface ISysService<TDto> : IService<TDto> where TDto : SysDto, new () {
        /// <summary>
        /// 批量修改状态
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        Result BatchUpdateStatus (int userID, IBatchDto dto, Status status);
    }

    /// <summary>
    /// 业务服务接口
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    public interface IBizService<TDto> : IService<TDto> where TDto : BizDto, new () {
        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Result BatchAudit (int userID, IBatchDto dto);
    }
}