using CcNetCore.Utils.Extensions;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// SQL排序
    /// </summary>
    public class SqlOrder {
        /// <summary>
        /// 字段名
        /// </summary>
        /// <value></value>
        public string Field { get; private set; }

        /// <summary>
        /// 是否升序
        /// </summary>
        /// <value></value>
        public bool Asc { get; private set; } = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="field">字段名</param>
        public SqlOrder (string field) {
            Field = field;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="field">字段名</param>
        /// <param name="asc">是否升序</param>
        public SqlOrder (string field, bool asc) {
            Field = field;
            Asc = asc;
        }

        /// <summary>
        /// 获取按升序排序的实例
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static SqlOrder[] OrderByAsc (params string[] fields) {
            if (fields.IsEmpty ()) {
                return null;
            }

            return fields.SelectEx (x => new SqlOrder (x)).ToArray ();
        }

        /// <summary>
        /// 获取按降序排序的实例
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static SqlOrder[] OrderByDesc (params string[] fields) {
            if (fields.IsEmpty ()) {
                return null;
            }

            return fields.SelectEx (x => new SqlOrder (x, false)).ToArray ();
        }
    }
}