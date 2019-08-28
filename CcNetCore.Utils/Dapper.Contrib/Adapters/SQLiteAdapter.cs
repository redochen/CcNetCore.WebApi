#pragma warning disable CS0168

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// The SQLite database adapter.
    /// </summary>
    public partial class SQLiteAdapter : ISqlAdapter {
        /// <summary>
        /// 是否存在表
        /// </summary>
        /// <param name="connection"><连接实例/param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public bool ExistsTable (IDbConnection connection, string tableName) {
            try {
                var sql = $"SELECT count(*) cnt FROM sqlite_master WHERE type='table' AND name='{tableName}';";
                var res = connection.QueryFirst<dynamic> (sql);
                return Convert.ToInt32 (res.cnt) > 0;
            } catch (Exception ex) {
                return false;
            }
        }

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
            var cmd = $"INSERT INTO {tableName} ({columnList}) VALUES ({parameterList}); SELECT last_insert_rowid() id";
            var multi = connection.QueryMultiple (cmd, entityToInsert, transaction, commandTimeout);

            var id = (int) multi.Read ().First ().id;
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
            sbSql.AppendFormat ("\"{0}\"", columnName);
        }

        /// <summary>
        /// gets a column equality to a parameter.
        /// </summary>
        /// <param name="column">The column's property info object.</param>
        public string GetColumnNameEqualsValue (PropertyInfo column) =>
            $"\"{column.GetColumnName()}\" = @{column.Name}";

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
            var typeName = attribute?.TypeName;
            var length = attribute?.Length;
            var varLength = attribute?.VarLength;
            var unicode = attribute?.Unicode ?? false;
            var defaultValue = attribute?.DefaultValue;

            sbSql.Append (columnName, Chars.空格);

            var columnType = typeName.IsValid () ? typeName : GetTypeName (type, length, varLength, unicode);
            sbSql.Append (columnType, Chars.空格);

            if (isExplicitKey) {
                sbSql.Append ("PRIMARY KEY", Chars.空格);
            } else if (isKey) {
                sbSql.Append ("UNIQUE", Chars.空格);
            }

            if (autoIncrement) {
                sbSql.Append ("AUTOINCREMENT", Chars.空格);
            }

            if (isKey || isNotNull) {
                sbSql.Append ("NOT NULL", Chars.空格);
            }

            if (defaultValue != null) {
                if (columnType.Contains ("char")) {
                    sbSql.AppendFormat ("DEFAULT '{0}'", defaultValue.ToString (), Chars.空格);
                } else {
                    sbSql.AppendFormat ("DEFAULT {0}", defaultValue, Chars.空格);
                }
            }
        }

        /// <summary>
        /// 获取分页查询Sql
        /// </summary>
        /// <param name="sbSql"></param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">页码，从0开始</param>
        public void GetPageQuerySql (StringBuilder sbSql, int? pageSize, int? pageIndex) {
            var size = (pageSize ?? 0);
            var index = (pageIndex ?? -1);

            if (size <= 0 || index < 0) {
                return;
            }

            sbSql.Append ($" limit {size} offset {size * index}");
        }

        /// <summary>
        /// 获取字段类型名称
        /// </summary>
        /// <param name="type"></param>
        /// <param name="length">字段固定长度</param>
        /// <param name="varLength">字段不定长度（仅对字符型字段有效）</param>
        /// <param name="unicode">是否为unicode字符（仅对字符型字段有效）</param>
        /// <returns></returns>
        private static string GetTypeName (Type type, int? length, int? varLength, bool unicode) {
            var kvp = TypeMaps.FirstOrDefault (t => t.Value.Contains (type));
            if (kvp.Key.IsValid ()) {
                return kvp.Key;
            }

            if (typeof (Enum) == type.BaseType) {
                return "INTEGER";
            }

            if (typeof (DateTime) == type) {
                return "VARCHAR(50)";
            }

            var len = varLength ?? 0;
            if (len > 0) {
                return unicode ? $"NVARCHAR({len})" : $"VARCHAR({len})";
            }

            len = length ?? 0;
            if (len <= 0) {
                return "TEXT";
            }

            return unicode ? $"NCHAR({len})" : $"CHARACTER({len})";
        }

        private static readonly Dictionary<string, Type[]> TypeMaps = new Dictionary<string, Type[]> () {
            ["INTEGER"] = new Type[] { typeof (bool), typeof (int), typeof (Int64), typeof (Int16), typeof (uint), typeof (UInt64), typeof (UInt16) }, ["REAL"] = new Type[] { typeof (float), typeof (double), typeof (decimal) },
        };
    }
}