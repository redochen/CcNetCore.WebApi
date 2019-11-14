using System.Data;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// SQL命令类
    /// </summary>
    public abstract class Command {
        /// <summary>
        /// 命令动作
        /// </summary>
        /// <value></value>
        public abstract string Verb { get; }

        /// <summary>
        /// 连接对象
        /// </summary>
        /// <value></value>
        public IDbConnection Connection { get; set; }

        /// <summary>
        /// 事务对象
        /// </summary>
        /// <value></value>
        public IDbTransaction Transaction { get; set; }

        /// <summary>
        /// 超时秒数
        /// </summary>
        /// <value></value>
        public int? TimeoutSeconds { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="command"></param>
        public Command (Command command) {
            if (command != null) {
                //Verb = command.Verb;
                Connection = command.Connection;
                Transaction = command.Transaction;
                TimeoutSeconds = command.TimeoutSeconds;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        public Command (IDbConnection connection) {
            Connection = connection;
            Transaction = null;
            TimeoutSeconds = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="timeoutSeconds">超时秒数</param>
        public Command (IDbConnection connection, int timeoutSeconds) {
            Connection = connection;
            Transaction = null;
            TimeoutSeconds = timeoutSeconds;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="transaction">事务对象</param>
        public Command (IDbConnection connection, IDbTransaction transaction) {
            Connection = connection;
            Transaction = transaction;
            TimeoutSeconds = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="transaction">事务对象</param>
        /// <param name="timeoutSeconds">超时秒数</param>
        public Command (IDbConnection connection, IDbTransaction transaction, int timeoutSeconds) {
            Connection = connection;
            Transaction = transaction;
            TimeoutSeconds = timeoutSeconds;
        }
    }
}