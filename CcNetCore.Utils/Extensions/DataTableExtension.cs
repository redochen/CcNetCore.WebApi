using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CcNetCore.Utils.Extensions {
    /// <summary>
    /// DataTable扩展类
    /// </summary>
    public static class DataTableExtension {
        /// <summary>
        /// 是否有数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool HasRows (this DataTable table) =>
            (table != null && table.Rows.Count > 0);

        /// <summary>
        /// 是否有列
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool HasColumns (this DataTable table) =>
            (table != null && table.Columns.Count > 0);

        /// <summary>
        /// 是否包含数据行
        /// </summary>
        /// <param name="table"></param>
        /// <param name="row">数据行</param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool FindRow (this DataTable table, DataRow row, out int index) {
            index = -1;

            if (!table.HasRows () || null == row) {
                return false;
            }

            for (int c = 0, count = row.Table.Columns.Count; c < count; ++c) {
                var name = row.Table.Columns[c].ColumnName;
                if (!table.Columns.Contains (name)) {
                    return false;
                }
            }

            for (int r = 0; r < table.Rows.Count; ++r) {
                for (int c = 0, count = table.Columns.Count; c < count; ++c) {
                    var name = table.Columns[c].ColumnName;
                    if (!table.Rows[r][name].EqualsEx (row[name])) {
                        break;
                    }

                    if (c == count - 1) {
                        index = r;
                        return true;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 替换DataTable的字段名
        /// </summary>
        /// <param name="table"></param>
        /// <param name="oldColumnName"></param>
        /// <param name="newColumnName"></param>
        public static void ReplaceColumnName (this DataTable table, string oldColumnName, string newColumnName) {
            if (!table.HasColumns () || !oldColumnName.IsValid () || !newColumnName.IsValid ()) {
                return;
            }

            if (table.Columns.Contains (oldColumnName)) {
                table.Columns[oldColumnName].ColumnName = newColumnName;
            }
        }

        /// <summary>
        /// 深复制数据表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable DeepCopy (this DataTable dt) {
            if (null == dt) {
                return null;
            }

            var result = dt.Clone ();
            dt.Rows.ForEach<DataRow> (dr => result.Rows.Add (dr.ItemArray));

            return result;
        }

        /// <summary>
        /// 获取第一行
        /// </summary>
        /// <param name="table"></param>
        /// <param name="predicate">过滤方法</param>
        /// <returns></returns>
        public static DataRow GetFirstRow (this DataTable table, Func<DataRow, bool> predicate = null) => table.Rows.FirstOrDefault (predicate);

        /// <summary>
        /// 获取最后一行
        /// </summary>
        /// <param name="table"></param>
        /// <param name="predicate">过滤方法</param>
        /// <returns></returns>
        public static DataRow GetLastRow (this DataTable table, Func<DataRow, bool> predicate = null) => table.Rows.LastOrDefault (predicate);

        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataRow AddRow (this DataTable table) {
            if (null == table) {
                return null;
            }

            DataRow dr = table.NewRow ();
            table.Rows.Add (dr);

            return dr;
        }

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        public static void AddColumns (this DataTable table, params string[] columns) => table.AddColumns (columns?.ToList ());

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        public static void AddColumns (this DataTable table, IEnumerable<string> columns) {
            if (columns.IsEmpty ()) {
                return;
            }

            foreach (var column in columns) {
                if (!table.Columns.Contains (column)) {
                    table.Columns.Add (column);
                }
            }
        }

        /// <summary>
        /// 选择数据列
        /// </summary>
        /// <param name="table"></param>
        /// <param name="htAliasMap">字段别名映射关系</param>
        /// <param name="includeFields">指定的字段是包含关系还是排除关系</param>
        /// <param name="fields">要包含或排除的字段（必须是经过别名映射之后的字段）</param>
        /// <returns></returns>
        public static DataTable SelectColumns (this DataTable table, Hashtable htAliasMap = null, bool includeFields = false, params string[] fields) => table.SelectEx (string.Empty, htAliasMap, includeFields, fields);

        /// <summary>
        /// 选择数据行
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where">行过滤条件</param>
        /// <param name="htAliasMap">字段别名映射关系</param>
        /// <returns></returns>
        public static DataTable SelectRows (this DataTable table, string where, Hashtable htAliasMap = null) => table.SelectEx (where, htAliasMap);

        /// <summary>
        /// 扩展选择方法
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where">行过滤条件</param>
        /// <param name="htAliasMap">字段别名映射关系</param>
        /// <param name="includeFields">指定的字段是包含关系还是排除关系</param>
        /// <param name="fields">要包含或排除的字段（必须是经过别名映射之后的字段）</param>
        /// <returns></returns>
        public static DataTable SelectEx (this DataTable table, string where, Hashtable htAliasMap = null, bool includeFields = false, params string[] fields) {
            DataTable result = null;

            if (null == table) {
                return result;
            }

            result = new DataTable ();

            #region 设置字段
            if (fields.IsEmpty ()) {
                if (null == htAliasMap) {
                    htAliasMap = new Hashtable ();
                }

                foreach (DataColumn dc in table.Columns) {
                    if (!htAliasMap.ContainsKey (dc.ColumnName)) {
                        htAliasMap.Add (dc.ColumnName, dc.ColumnName);
                    }

                    result.Columns.Add (htAliasMap[dc.ColumnName].ToString ());
                }
            } else {
                fields.ToList ().ForEach (x => result.Columns.Add (x));
            }
            #endregion

            #region 过滤数据
            DataRow[] rows = null;

            try {
                if (where.IsValid (true)) {
                    rows = table.Select (where);
                } else {
                    rows = table.Select ();
                }
            } catch { }
            #endregion

            if (rows.IsEmpty ()) {
                return result;
            }

            foreach (DataRow row in rows) {
                var dr = result.NewRow ();

                foreach (DictionaryEntry entry in htAliasMap) {
                    if (!row.Table.Columns.Contains (entry.Key.ToString ()) ||
                        !dr.Table.Columns.Contains (entry.Value.ToString ())) {
                        continue;
                    }

                    dr[entry.Value.ToString ()] = row[entry.Key.ToString ()];
                }

                result.Rows.Add (dr);
            }

            return result;
        }

        /// <summary>
        /// 确认数据表有指定字段
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnName">字段名称</param>
        public static void EnsureHasColumn (this DataTable table, string columnName) {
            if (null == table || !columnName.IsValid (true)) {
                return;
            }

            if (!table.Columns.Contains (columnName)) {
                table.Columns.Add (columnName);
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="traveller"></param>
        public static void ForEach<T> (this InternalDataCollectionBase collection, Action<T /*value*/> traveller) {
            foreach (T current in collection) {
                traveller (current);
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="traveller"></param>
        public static void ForEach<T> (this InternalDataCollectionBase collection, Action<int /*index*/ , T /*value*/> traveller) {
            var index = 0;
            foreach (T current in collection) {
                traveller (index, current);
                index++;
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="traveller">返回值：True=break；False=continue</param>
        public static void ForEach<T> (this InternalDataCollectionBase collection, Func<T /*value*/ , bool /*break*/> traveller) {
            foreach (T current in collection) {
                if (traveller (current)) {
                    break;
                }
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="traveller">返回值：True=break；False=continue</param>
        public static void ForEach<T> (this InternalDataCollectionBase collection, Func<int /*index*/ , T /*value*/ , bool /*break*/> traveller) {
            var index = 0;
            foreach (T current in collection) {
                if (traveller (index, current)) {
                    break;
                }

                index++;
            }
        }
    }
}