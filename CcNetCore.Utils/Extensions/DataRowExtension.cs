#pragma warning disable CS0168

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CcNetCore.Utils.Extensions {
    /// <summary>
    /// DataRow扩展类
    /// </summary>
    public static class DataRowExtension {
        /// <summary>
        /// 转成特定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static T ToObject<T> (this DataRow dataRow) {
            T t = Activator.CreateInstance<T> (); //创建实例
            var members = t.GetFieldsAndProperities (inherit: true, includeStatic: false); //取类的属性

            foreach (var member in members) {
                if (!dataRow.Table.Columns.Contains (member.Name)) {
                    continue;
                }

                var value = dataRow.GetValue (member.Name);
                t.SetMemberValue (member, value);
            }

            return t;
        }

        /// <summary>
        /// 获取列对象（DataTable列名不区分大小写）
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static DataColumn GetColumn (this DataRow dataRow, string columnName) {
            if (null == dataRow || !columnName.IsValid ()) {
                return null;
            }

            if (dataRow.Table.Columns.Count <= 0 || !dataRow.Table.Columns.Contains (columnName)) {
                return null;
            }

            return dataRow.Table.Columns[columnName];
        }

        /// <summary>
        /// 深复制数据行
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static DataRow DeepCopy (this DataRow dataRow) {
            if (null == dataRow) {
                return null;
            }

            DataRow result = null;

            try {
                result = dataRow.Table.NewRow ();
                result.ItemArray = (object[]) dataRow.ItemArray.Clone ();
            } catch (Exception ex) { }

            return result;
        }

        /// <summary>
        /// 合并数据行
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static DataRow MergeDataRow (this DataRow dataRow, DataRow other) {
            if (null == dataRow || null == other) {
                return dataRow;
            }

            foreach (DataColumn col in other.Table.Columns) {
                try {
                    if (!dataRow.Table.Columns.Contains (col.ColumnName)) {
                        dataRow.Table.Columns.Add (col.ColumnName);
                    }

                    dataRow.SetValue (col.ColumnName, other.GetValue (col.ColumnName));
                } catch { }
            }

            return dataRow;
        }

        /// <summary>
        /// 合并其他数据行
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public static DataRow MergeDataRows (this DataRow dataRow, params DataRow[] others) {
            if (null == dataRow || others.IsEmpty ()) {
                return dataRow;
            }

            foreach (var other in others) {
                dataRow.MergeDataRow (other);
            }

            return dataRow;
        }

        /// <summary>
        /// 更新数据行
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="other"></param>
        /// <param name="addIfNotExists">列不存在时是否添加</param>
        /// <returns></returns>
        public static DataRow UpdateDataRow (this DataRow dataRow, DataRow other, bool addIfNotExists = false) {
            if (null == dataRow || null == other) {
                return dataRow;
            }

            foreach (DataColumn col in other.Table.Columns) {
                dataRow.SetValue (col.ColumnName, other.GetValue (col), addIfNotExists);
            }

            return dataRow;
        }

        /// <summary>
        /// 更新数据行
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="obj">源对象</param>
        /// <param name="addIfNotExists">列不存在时是否添加</param>
        /// <param name="includeFields">指定的字段列表是否为包含关系（否则为排除关系）</param>
        /// <param name="fields">指定要包含或排除的字段列表</param>
        /// <returns></returns>
        public static DataRow UpdateDataRow (this DataRow dataRow, object obj,
            bool addIfNotExists = false, bool includeFields = false, List<string> fields = null) {
            if (null == dataRow || null == obj) {
                return dataRow;
            }

            var members = obj.GetFieldsAndProperities (inherit: true, includeStatic: true);
            if (members.IsEmpty ()) {
                return dataRow;
            }

            foreach (var member in members) {
                try {
                    if (!fields.IsEmpty ()) {
                        var include = fields.Contains (member.Name);
                        if (include ^ includeFields) {
                            continue;
                        }
                    }

                    var value = member.GetMemberValue (obj);
                    dataRow.SetValue (member.Name, value, addIfNotExists);
                } catch { }
            }

            return dataRow;
        }

        /// <summary>
        /// 复制字段值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="srcColumn">复制源字段</param>
        /// <param name="dstColumn">复制目标字段</param>
        /// <param name="addIfNotExists">列不存在时是否添加</param>
        public static void CopyColumnValue (this DataRow dataRow, string srcColumn, string dstColumn, bool addIfNotExists = false) {
            if (null == dataRow || !srcColumn.IsValid () || !dstColumn.IsValid ()) {
                return;
            }

            var value = dataRow.GetValue (srcColumn);
            dataRow.SetValue (dstColumn, value, addIfNotExists);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <param name="addIfNotExists">列不存在时是否添加</param>
        public static void SetValue (this DataRow dataRow, string columnName, object value, bool addIfNotExists = false) {
            try {
                if (null == dataRow || !columnName.IsValid () || null == value) {
                    return;
                }

                if (!dataRow.Table.Columns.Contains (columnName)) {
                    if (!addIfNotExists) {
                        return;
                    }

                    dataRow.Table.Columns.Add (columnName);
                }

                var dataType = dataRow.Table.Columns[columnName].DataType;
                if (value.GetType () != dataType) {
                    dataRow[columnName] = Convert.ChangeType (value, dataType);
                } else {
                    dataRow[columnName] = value;
                }
            } catch (Exception ex) { }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="dataColumn"></param>
        /// <param name="value"></param>
        /// <param name="addIfNotExists">列不存在时是否添加</param>
        public static void SetValue (this DataRow dataRow, DataColumn dataColumn, object value, bool addIfNotExists = false) {
            dataRow.SetValue (dataColumn?.ColumnName, value, addIfNotExists);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static object GetValue (this DataRow dataRow, string columnName) {
            var column = dataRow.GetColumn (columnName);
            if (column != null) {
                return dataRow[column];
            }

            return null;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="dataColumn">列对象</param>
        /// <returns></returns>
        public static object GetValue (this DataRow dataRow, DataColumn dataColumn) {
            if (null == dataRow || null == dataColumn) {
                return null;
            }

            return dataRow[dataColumn];
        }

        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static string GetString (this DataRow dataRow, string columnName) => dataRow.GetValue (columnName).ToStringEx ();

        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="dataColumn">列对象</param>
        /// <returns></returns>
        public static string GetString (this DataRow dataRow, DataColumn dataColumn) => dataRow.GetValue (dataColumn).ToStringEx ();

        /// <summary>
        /// 获取整形值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static int GetInt (this DataRow dataRow, string columnName) => dataRow.GetValue (columnName).ToInt ();

        /// <summary>
        /// 获取整形值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="dataColumn">列对象</param>
        /// <returns></returns>
        public static int GetInt (this DataRow dataRow, DataColumn dataColumn) => dataRow.GetValue (dataColumn).ToInt ();

        /// <summary>
        /// 获取长整形值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static long GetLong (this DataRow dataRow, string columnName) => dataRow.GetValue (columnName).ToLong ();

        /// <summary>
        /// 获取长整形值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="dataColumn">列对象</param>
        /// <returns></returns>
        public static long GetLong (this DataRow dataRow, DataColumn dataColumn) => dataRow.GetValue (dataColumn).ToLong ();

        /// <summary>
        /// 获取Double值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static double GetDouble (this DataRow dataRow, string columnName) => dataRow.GetValue (columnName).ToDouble ();

        /// <summary>
        /// 获取Double值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="dataColumn">列对象</param>
        /// <returns></returns>
        public static double GetDouble (this DataRow dataRow, DataColumn dataColumn) => dataRow.GetValue (dataColumn).ToDouble ();

        /// <summary>
        /// 获取Decimal值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static decimal GetDecimal (this DataRow dataRow, string columnName) => dataRow.GetValue (columnName).ToDecimal ();

        /// <summary>
        /// 获取Decimal值
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="dataColumn">列对象</param>
        /// <returns></returns>
        public static decimal GetDecimal (this DataRow dataRow, DataColumn dataColumn) => dataRow.GetValue (dataColumn).ToDecimal ();

        /// <summary>
        /// 根据指定的列判断数据行是否有效
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnFlags">字段检测标识集合</param>
        /// <param name="includeColumns">是否为包含列（否则为排除列）</param>
        /// <param name="columns">要包含或排除的列（为空时判断所有列）。假如数据行没有要包含的列，则视为无效的数据行</param>
        /// <returns></returns>
        public static bool IsValid (this DataRow dataRow, Dictionary<string, CheckValidFlag> columnFlags = null, bool includeColumns = false, params string[] columns) {
            return dataRow.IsValid (columnFlags, columns?.ToList (), includeColumns);
        }

        /// <summary>
        /// 根据指定的列判断数据行是否有效
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnFlags">字段检测标识集合</param>
        /// <param name="columns">要包含或排除的列（为空时判断所有列）。假如数据行没有要包含的列，则视为无效的数据行</param>
        /// <param name="includeColumns">是否为包含列（否则为排除列）</param>
        /// <returns></returns>
        public static bool IsValid (this DataRow dataRow, Dictionary<string, CheckValidFlag> columnFlags = null, List<string> columns = null, bool includeColumns = false) {
            if (null == dataRow || dataRow.Table.Columns.Count <= 0) {
                return false;
            }

            //数据行没有要包含的列，则视为无效的数据行
            if (includeColumns && !columns.IsEmpty ()) {
                if (columns.Any (x => !dataRow.Table.Columns.Contains (x))) {
                    return false;
                }
            }

            var hasColumn = false;

            foreach (DataColumn col in dataRow.Table.Columns) {
                if (!columns.IsEmpty ()) {
                    var found = columns.Contains (col.ColumnName);
                    if (found ^ includeColumns) {
                        continue;
                    }
                }

                hasColumn = true;

                var flags = CheckValidFlag.Default;
                if (columnFlags != null && columnFlags.ContainsKey (col.ColumnName)) {
                    flags = columnFlags[col.ColumnName];
                }

                if (!dataRow[col].IsValid (flags)) {
                    return false;
                }
            }

            return hasColumn;
        }

        /// <summary>
        /// 改变日期时间格式
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="column">字段名称</param>
        /// <param name="format">格式</param>
        public static void ChangeDateTimeFormat (this DataRow dataRow, string columnName, string format) {
            if (null == dataRow) {
                return;
            }

            var value = dataRow.GetString (columnName);
            if (!value.IsValid ()) {
                return;
            }

            value = value.TryDateTime ()?.ToString (format);

            var column = dataRow.Table.Columns[columnName];
            if (column.DataType.Equals (typeof (string))) {
                dataRow[columnName] = value;
            } else if (column.DataType.Equals (typeof (DateTime))) {
                dataRow[columnName] = value.ToDateTime ();
            }
        }
    }
}