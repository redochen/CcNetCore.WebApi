using System;
using System.Data;
using System.Linq;
using System.Text;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// The Dapper.Contrib extensions for Dapper
    /// </summary>
    public static partial class SqlMapperExtensions {
        /// <summary>
        /// Delete entity in table "Ts".
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToDelete">Entity to delete</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>true if deleted, false if not found</returns>
        public static bool Delete<T> (this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class {
            if (entityToDelete == null) {
                throw new ArgumentException ("Cannot Delete null Object", nameof (entityToDelete));
            }

            var type = typeof (T);

            if (type.IsArray) {
                type = type.GetElementType ();
            } else if (type.IsGenericType) {
                //var typeInfo = type.GetTypeInfo();
                //bool implementsGenericIEnumerableOrIsGenericIEnumerable =
                //    typeInfo.ImplementedInterfaces.Any(ti => ti.IsGenericType() && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
                //    typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);

                //if (implementsGenericIEnumerableOrIsGenericIEnumerable)
                {
                    type = type.GetGenericArguments () [0];
                }
            }

            var keyProperties = KeyPropertiesCache (type).ToList (); //added ToList() due to issue #418, must work on a list copy
            var explicitKeyProperties = ExplicitKeyPropertiesCache (type);
            if (keyProperties.Count == 0 && explicitKeyProperties.Count == 0) {
                throw new ArgumentException ("Entity must have at least one [Key] or [ExplicitKey] property");
            }

            var name = GetTableName (type);
            keyProperties.AddRange (explicitKeyProperties);

            var sbSql = new StringBuilder ();
            sbSql.AppendFormat ("delete from {0} where ", name);

            var adapter = GetFormatter (connection);

            for (var i = 0; i < keyProperties.Count; i++) {
                var property = keyProperties[i];
                sbSql.Append (adapter.GetColumnMatchesValue (property.GetColumnName (),
                    property.Name, MatchType.Equal)); //fix for issue #336
                if (i < keyProperties.Count - 1) {
                    sbSql.Append (" and ");
                }
            }

            var deleted = connection.Execute (sbSql.ToString (), entityToDelete, transaction, commandTimeout);
            return deleted > 0;
        }

        /// <summary>
        /// Delete all entities in the table related to the type T.
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>true if deleted, false if none found</returns>
        public static bool DeleteAll<T> (this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class {
            var type = typeof (T);
            var name = GetTableName (type);
            var statement = $"delete from {name}";
            var deleted = connection.Execute (statement, null, transaction, commandTimeout);

            //总是返回成功
            return true;
            //return deleted > 0;
        }

        /// <summary>
        /// 根据条件批量删除数据
        /// </summary>
        /// <param name="cmd">删除命令实例</param>
        /// <param name="predicate">WHERE匹配谓词实例</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool DeleteWhere<T> (this DeleteCommand cmd, SqlPredicate<T> predicate)
        where T : class, new () {
            var (sbSql, parameters) = cmd.GetSql (predicate);

            var deleted = cmd.Connection.Execute (sbSql, parameters,
                cmd.Transaction, commandTimeout : cmd.TimeoutSeconds);
            return deleted > 0;
        }
    }
}