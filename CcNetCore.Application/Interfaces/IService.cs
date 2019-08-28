using CcNetCore.Application.Models;

namespace CcNetCore.Application.Interfaces {
    /// <summary>
    /// 服务接口
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IService<TModel> where TModel : BaseModel, new () {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        BaseResult Create (int userID, ICreateModel model);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        BaseResult Update (int userID, IUpdateModel model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        BaseResult Delete (int userID, IDeleteModel model);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="model"></param>
        /// <returns></returns>
        BaseResult BatchDelete (int userID, IBatchDeleteModel model);

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        PageQueryResult<TModel> Query (PageQueryModel<TModel> model);
    }
}