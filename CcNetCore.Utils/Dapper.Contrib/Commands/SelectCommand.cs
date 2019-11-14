using System.Collections.Generic;
using System.Data;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// 查询命令类
    /// </summary>
    public class SelectCommand : Command {
        /// <summary>
        /// 命令动作
        /// </summary>
        /// <value></value>
        public override string Verb => GetSelectVerb ();

        /// <summary>
        /// 每页行数
        /// </summary>
        /// <value></value>
        public int? PageSize { get; set; }

        /// <summary>
        /// 页码，从0开始
        /// </summary>
        /// <value></value>
        public int? PageIndex { get; set; }

        /// <summary>
        /// 选择字段集合
        /// </summary>
        /// <value></value>
        public string[] SelectFields { get; set; }

        /// <summary>
        /// 排序字段集合
        /// </summary>
        /// <value></value>
        public SqlOrder[] SortFields { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public SelectCommand (Command command):
            base (command) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        public SelectCommand (IDbConnection connection):
            base (connection) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="timeoutSeconds">超时秒数</param>
        public SelectCommand (IDbConnection connection, int timeoutSeconds):
            base (connection, timeoutSeconds) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="transaction">事务对象</param>
        public SelectCommand (IDbConnection connection, IDbTransaction transaction):
            base (connection, transaction) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="transaction">事务对象</param>
        /// <param name="timeoutSeconds">超时秒数</param>
        public SelectCommand (IDbConnection connection, IDbTransaction transaction, int timeoutSeconds):
            base (connection, transaction, timeoutSeconds) { }

        /// <summary>
        /// 设置分页信息
        /// </summary>
        /// <param name="pageSize">每页行数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        public SelectCommand SetPageInfo (int pageSize, int pageIndex) {
            PageSize = pageSize;
            PageIndex = pageIndex;
            return this;
        }

        /// <summary>
        /// 获取按升序排序的实例
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SelectCommand OrderByAsc (params string[] fields) {
            SortFields = SqlOrder.OrderByAsc (fields);
            return this;
        }

        /// <summary>
        /// 获取按降序排序的实例
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SelectCommand OrderByDesc (params string[] fields) {
            SortFields = SqlOrder.OrderByDesc (fields);
            return this;
        }

        /// <summary>
        /// 获取选择动作
        /// </summary>
        /// <returns></returns>
        private string GetSelectVerb () {
            string selectFields;

            if (SelectFields.IsEmpty ()) {
                selectFields = "*";
            } else {
                selectFields = string.Join (Chars.逗号, SelectFields);
            }

            return $"select {selectFields} from";
        }
    }
}