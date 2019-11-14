using System.Collections.Generic;
using System.Data;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// 更新命令类
    /// </summary>
    public class UpdateCommand : Command {
        /// <summary>
        /// 命令动作
        /// </summary>
        /// <value></value>
        public override string Verb => "update";

        /// <summary>
        /// 更新字段集合
        /// </summary>
        /// <value></value>
        public string[] UpdateFields { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public UpdateCommand (Command command):
            base (command) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        public UpdateCommand (IDbConnection connection):
            base (connection) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="timeoutSeconds">超时秒数</param>
        public UpdateCommand (IDbConnection connection, int timeoutSeconds):
            base (connection, timeoutSeconds) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="transaction">事务对象</param>
        public UpdateCommand (IDbConnection connection, IDbTransaction transaction):
            base (connection, transaction) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="transaction">事务对象</param>
        /// <param name="timeoutSeconds">超时秒数</param>
        public UpdateCommand (IDbConnection connection, IDbTransaction transaction, int timeoutSeconds):
            base (connection, transaction, timeoutSeconds) { }
    }
}