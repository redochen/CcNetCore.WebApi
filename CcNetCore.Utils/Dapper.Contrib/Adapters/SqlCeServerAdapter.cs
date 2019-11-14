using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// The SQL Server Compact Edition database adapter.
    /// </summary>
    public partial class SqlCeServerAdapter : ISqlAdapter {
        /// <summary>
        /// 是否存在表
        /// </summary>
        /// <param name="connection"><连接实例/param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public bool ExistsTable (IDbConnection connection, string tableName) => true;

        /// <summary>
        /// Inserts <paramref name="entityToInsert"/> into the database, returning the Id of the row created.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <param name="transaction">The transaction to use.</param>
        /// <param name="commandTimeout">The command timeout to use.</param>
        /// <param name="tableName">The table to insert into.</param>
        /// <param name="columnList">The columns to set with this insert.</param>
        /// <param name="parameterList">The parameters to set for this insert.</param>
        /// <param name="keyProperties">The key columns in this table.</param>
        /// <param name="entityToInsert">The entity to insert.</param>
        /// <returns>The Id of the row created.</returns>
        public int Insert (IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert) {
            var cmd = $"insert into {tableName} ({columnList}) values ({parameterList})";
            connection.Execute (cmd, entityToInsert, transaction, commandTimeout);
            var r = connection.Query ("select @@IDENTITY id", transaction : transaction, commandTimeout : commandTimeout).ToList ();

            if (r[0].id == null) {
                return 0;
            }

            var id = (int) r[0].id;

            var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray ();
            if (propertyInfos.Length == 0) {
                return id;
            }

            var idProperty = propertyInfos[0];
            idProperty.SetValue (entityToInsert, Convert.ChangeType (id, idProperty.PropertyType), null);

            return id;
        }

        /// <summary>
        /// Adds the name of a column.
        /// </summary>
        /// <param name="sbSql">The string builder  to append to.</param>
        /// <param name="columnName">The column name.</param>
        public void AppendColumnName (StringBuilder sbSql, string columnName) {
            sbSql.AppendFormat ("[{0}]", columnName);
        }

        /// <summary>
        /// gets a column matches a parameter.
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="paraName">参数名</param>
        /// <param name="matchType">匹配类型</param>
        public string GetColumnMatchesValue (string columnName, string paraName, MatchType matchType = MatchType.Equal) {
            var expression = MatchExps[matchType];
            return expression.Replace ("Column", columnName).Replace ("Param", paraName);
        }

        /// <summary>
        /// 添加列定义
        /// </summary>
        /// <param name="sbSql"></param>
        /// <param name="columnName">列名</param>
        /// <param name="type">类型</param>
        /// <param name="attribute">字段属性</param>
        /// <param name="isExplicitKey">是否为主键</param>
        /// <param name="isKey">是否为键</param>
        /// <param name="isNotNull">是否非空</param>
        /// <param name="autoIncrement">是否自动增长列</param>
        public void AppendColumnDefination (StringBuilder sbSql, string columnName, Type type,
            ColumnAttribute attribute, bool isExplicitKey, bool isKey, bool isNotNull, bool autoIncrement) {
            //TODO:
        }

        /// <summary>
        /// 获取排序的sql
        /// </summary>
        /// <param name="sbSql"></param>
        /// <param name="sortFields">排序字段集合</param>
        public void GetOrderBySql (StringBuilder sbSql, IEnumerable<SqlOrder> sortFields) {
            //TODO:
        }

        /// <summary>
        /// 获取分页查询Sql
        /// </summary>
        /// <param name="sbSql"></param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        public void GetPageQuerySql (StringBuilder sbSql, int? pageSize, int? pageIndex) {
            //TODO:
        }

        private static readonly Dictionary<MatchType, string> MatchExps =
            new Dictionary<MatchType, string> {
                [MatchType.Equal] = "[Column] = @Param",
                [MatchType.NotEqual] = "[Column] <> @Param",
                [MatchType.Greater] = "[Column] > @Param",
                [MatchType.GreaterOrEqual] = "[Column] >= @Param",
                [MatchType.Less] = "[Column] < @Param",
                [MatchType.LessOrEqual] = "[Column] <= @Param",
                [MatchType.In] = "[Column] in @Param",
                [MatchType.NotIn] = "[Column] not in @Param",
                [MatchType.Like] = "[Column] like @Param",
                [MatchType.NotLike] = "[Column] not like @Param",
                [MatchType.BeginsWith] = "[Column] like @Param",
                [MatchType.NotBeginsWith] = "[Column] not like @Param",
                [MatchType.EndsWith] = "[Column] like @Param",
                [MatchType.NotEndsWith] = "[Column] not like @Param",
            };
    }
}