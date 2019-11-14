using System.Data;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// 删除命令类
    /// </summary>
    public class DeleteCommand : Command {
        /// <summary>
        /// 命令动作
        /// </summary>
        /// <value></value>
        public override string Verb => "delete from";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public DeleteCommand (Command command):
            base (command) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        public DeleteCommand (IDbConnection connection):
            base (connection) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="timeoutSeconds">超时秒数</param>
        public DeleteCommand (IDbConnection connection, int timeoutSeconds):
            base (connection, timeoutSeconds) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="transaction">事务对象</param>
        public DeleteCommand (IDbConnection connection, IDbTransaction transaction):
            base (connection, transaction) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="transaction">事务对象</param>
        /// <param name="timeoutSeconds">超时秒数</param>
        public DeleteCommand (IDbConnection connection, IDbTransaction transaction, int timeoutSeconds):
            base (connection, transaction, timeoutSeconds) { }
    }
}