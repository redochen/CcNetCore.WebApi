namespace CcNetCore.Utils.Interfaces {
    /// <summary>
    /// 单例接口，用于Autofac自动注册时，查找所有依赖该接口的实现。
    /// </summary>
    public interface ISingletonInstance { }

    /// <summary>
    /// 普通接口（每次创建新实例），用于Autofac自动注册时，查找所有依赖该接口的实现
    /// </summary>
    public interface ITransientInstance { }
}