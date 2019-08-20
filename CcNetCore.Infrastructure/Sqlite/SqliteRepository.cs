using System.Data;
using System.Data.SQLite;
using CcNetCore.Domain;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;

namespace CcNetCore.Infrastructure {
    /// <summary>
    /// SQLite仓储基类
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SqliteRepository<TDto, TEntity> : BaseRepository<TDto, TEntity>
        where TDto : BaseDto, new ()
    where TEntity : BaseEntity, new () {
        /// <summary>
        /// 自动装载属性（必须为public，否则自动装载失败）
        /// </summary>
        /// <value></value>
        public IDbContext _dbContext { get; set; }

        /// <summary>
        /// 获取连接实例
        /// </summary>
        /// <returns></returns>
        public override IDbConnection GetConnection () =>
            new SQLiteConnection (_dbContext.GetConnectionString ());
    }
}