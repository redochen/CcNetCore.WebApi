namespace CcNetCore.Application.Interfaces {
    /// <summary>
    /// 创建操作模型接口
    /// </summary>
    public interface ICreateModel { }

    /// <summary>
    /// 更新操作模型接口
    /// </summary>
    public interface IUpdateModel { }

    /// <summary>
    /// 删除操作模型接口
    /// </summary>
    public interface IDeleteModel { }

    /// <summary>
    /// 批量删除操作模型接口
    /// </summary>
    public interface IBatchDeleteModel {
        /// <summary>
        /// 惟一标识集合
        /// </summary>
        /// <value></value>
        string[] Uid { get; }
    }
}