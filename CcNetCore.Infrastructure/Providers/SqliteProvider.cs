using System.Data;
using System.Data.SQLite;
using CcNetCore.Domain;
using CcNetCore.Utils.Helpers;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Infrastructure.Providers {
    /// <summary>
    /// SQLite数据供应类
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class SqliteProvider : IDbProvider, ISingletonInstance {
        /// <summary>
        /// 自动装载属性（必须为public，否则自动装载失败）
        /// </summary>
        /// <value></value>
        public IDbContext _dbContext { get; set; }

        /// <summary>
        /// 获取连接实例
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection () =>
            new SQLiteConnection (_dbContext.GetConnectionString ());
    }
}