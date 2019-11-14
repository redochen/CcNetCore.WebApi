namespace CcNetCore.Application.Interfaces {
    /// <summary>
    /// 创建操作Dto接口
    /// </summary>
    public interface ICreateDto { }

    /// <summary>
    /// 更新操作Dto接口
    /// </summary>
    public interface IUpdateDto { }

    /// <summary>
    /// 删除操作Dto接口
    /// </summary>
    public interface IDeleteDto { }

    /// <summary>
    /// 批量操作Dto接口
    /// </summary>
    public interface IBatchDto {
        /// <summary>
        /// 惟一标识集合
        /// </summary>
        /// <value></value>
        string[] Uid { get; }
    }
}