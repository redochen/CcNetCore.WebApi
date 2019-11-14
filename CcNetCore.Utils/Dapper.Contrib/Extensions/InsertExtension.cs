using System.Data;
using System.Linq;
using System.Text;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// The Dapper.Contrib extensions for Dapper
    /// </summary>
    public static partial class SqlMapperExtensions {
        /// <summary>
        /// Inserts an entity into table "Ts" and returns identity id or number of inserted rows if inserting a list.
        /// </summary>
        /// <typeparam name="T">The type to insert.</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToInsert">Entity to insert, can be list of entities</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>Identity of inserted entity, or number of inserted rows if inserting a list</returns>
        public static long Insert<T> (this IDbConnection connection, T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class {
            var isList = false;

            var type = typeof (T);

            if (type.IsArray) {
                isList = true;
                type = type.GetElementType ();
            } else if (type.IsGenericType) {
                //var typeInfo = type.GetTypeInfo();
                //bool implementsGenericIEnumerableOrIsGenericIEnumerable =
                //    typeInfo.ImplementedInterfaces.Any(ti => ti.IsGenericType() && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
                //    typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);

                //if (implementsGenericIEnumerableOrIsGenericIEnumerable)
                {
                    isList = true;
                    type = type.GetGenericArguments () [0];
                }
            }

            var tableName = GetTableName (type);
            var sbColumnList = new StringBuilder (null);
            var allProperties = TypePropertiesCache (type);
            var keyProperties = KeyPropertiesCache (type);
            var autoIncrementProperties = AutoIncrementPropertiesCache (type);
            var ignoredProperties = IgnoredPropertiesCache (type);
            var allPropertiesExceptKeyAndComputed = allProperties
                .Except (autoIncrementProperties)
                .Except (ignoredProperties).ToList ();

            var adapter = GetFormatter (connection);

            for (var i = 0; i < allPropertiesExceptKeyAndComputed.Count; i++) {
                var columnName = GetColumnName (allPropertiesExceptKeyAndComputed[i]);
                adapter.AppendColumnName (sbColumnList, columnName); //fix for issue #336
                if (i < allPropertiesExceptKeyAndComputed.Count - 1) {
                    sbColumnList.Append (", ");
                }
            }

            var sbParameterList = new StringBuilder (null);
            for (var i = 0; i < allPropertiesExceptKeyAndComputed.Count; i++) {
                var property = allPropertiesExceptKeyAndComputed[i];
                sbParameterList.AppendFormat ("@{0}", property.Name);
                if (i < allPropertiesExceptKeyAndComputed.Count - 1) {
                    sbParameterList.Append (", ");
                }
            }

            int returnVal;

            var wasClosed = connection.State == ConnectionState.Closed;
            if (wasClosed) {
                connection.Open ();
            }

            if (!isList) {
                //single entity
                returnVal = adapter.Insert (connection, transaction, commandTimeout, tableName,
                    sbColumnList.ToString (), sbParameterList.ToString (), keyProperties, entityToInsert);
            } else {
                //insert list of entities
                var cmd = $"insert into {tableName} ({sbColumnList}) values ({sbParameterList})";
                returnVal = connection.Execute (cmd, entityToInsert, transaction, commandTimeout);
            }

            if (wasClosed) {
                connection.Close ();
            }

            return returnVal;
        }
    }
}