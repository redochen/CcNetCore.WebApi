#pragma warning disable CS0168
#define NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CcNetCore.Utils.Extensions;

using Schema = System.ComponentModel.DataAnnotations.Schema;

#if NETSTANDARD1_3
using DataException = System.InvalidOperationException;
#else
#endif

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// The Dapper.Contrib extensions for Dapper
    /// </summary>
    public static partial class SqlMapperExtensions {
        /// <summary>
        /// Defined a proxy object with a possibly dirty state.
        /// </summary>
        public interface IProxy //must be kept public
        {
            /// <summary>
            /// Whether the object has been changed.
            /// </summary>
            bool IsDirty { get; set; }
        }

        ///// <summary>
        ///// Defines a table name mapper for getting table names from types.
        ///// </summary>
        //public interface ITableNameMapper
        //{
        //    /// <summary>
        //    /// Gets a table name from a given <see cref="Type"/>.
        //    /// </summary>
        //    /// <param name="type">The <see cref="Type"/> to get a name from.</param>
        //    /// <returns>The table name for the given <paramref name="type"/>.</returns>
        //    string GetTableName(Type type);
        //}

        /// <summary>
        /// The function to get a database type from the given <see cref="IDbConnection"/>.
        /// </summary>
        /// <param name="connection">The connection to get a database type name from.</param>
        public delegate string GetDatabaseTypeDelegate (IDbConnection connection);

        /// <summary>
        /// The function to get a a table name from a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get a table name for.</param>
        public delegate string TableNameMapperDelegate (Type type);

        private static readonly ISqlAdapter DefaultAdapter = new SqlServerAdapter ();
        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary = new Dictionary<string, ISqlAdapter> {
            ["sqlconnection"] = new SqlServerAdapter (),
            ["sqlceconnection"] = new SqlCeServerAdapter (),
            ["npgsqlconnection"] = new PostgresAdapter (),
            ["sqliteconnection"] = new SQLiteAdapter (),
            ["mysqlconnection"] = new MySqlAdapter (),
            ["fbconnection"] = new FbAdapter ()
        };

        /// <summary>
        /// Specifies a custom callback that detects the database type instead of relying on the default strategy (the name of the connection type object).
        /// Please note that this callback is global and will be used by all the calls that require a database specific adapter.
        /// </summary>
        public static GetDatabaseTypeDelegate GetDatabaseType;

        private static ISqlAdapter GetFormatter (IDbConnection connection) {
            var name = GetDatabaseType?.Invoke (connection).ToLower () ??
                connection.GetType ().Name.ToLower ();

            return !AdapterDictionary.ContainsKey (name) ?
                DefaultAdapter :
                AdapterDictionary[name];
        }

        private static List<PropertyInfo> GetAttributeProperties<TAttribute> (Type type)
        where TAttribute : Attribute {
            var typeAttr = typeof (TAttribute);
            return TypePropertiesCache (type).Where (p => p.IsDefined (typeAttr, true))?.ToList ();
        }

        private static TypeConverter GetTypeConverter (TypeConverterAttribute attribute) {
            if (null == attribute) {
                return null;
            }

            if (TypeConverters.TryGetValue (attribute, out TypeConverter converter)) {
                return converter;
            }

            converter = (TypeConverter) Activator.CreateInstance (
                Type.GetType (attribute.ConverterTypeName));

            TypeConverters[attribute] = converter;
            return converter;
        }

        private static bool IsWriteable (PropertyInfo pi) {
            var attributes = pi.GetCustomAttributes (typeof (WriteAttribute), false).AsList ();
            if (attributes.Count != 1) {
                return true;
            }

            var writeAttribute = (WriteAttribute) attributes[0];
            return writeAttribute.Write;
        }

        private static PropertyInfo GetSingleKey<T> (string method) {
            var type = typeof (T);
            var keys = KeyPropertiesCache (type);
            var explicitKeys = ExplicitKeyPropertiesCache (type);
            var keyCount = keys.Count + explicitKeys.Count;
            if (keyCount > 1) {
                throw new DataException ($"{method}<T> only supports an entity with a single [Key] or [ExplicitKey] property. [Key] Count: {keys.Count}, [ExplicitKey] Count: {explicitKeys.Count}");
            }

            if (keyCount == 0) {
                throw new DataException ($"{method}<T> only supports an entity with a [Key] or an [ExplicitKey] property");
            }

            return keys.Count > 0 ? keys[0] : explicitKeys[0];
        }

        /// <summary>
        /// Specify a custom table name mapper based on the POCO type name
        /// </summary>
        public static TableNameMapperDelegate TableNameMapper;

        private static string GetTableName (Type type) {
            if (TypeTableName.TryGetValue (type.TypeHandle, out string name)) {
                return name;
            }

            if (TableNameMapper != null) {
                name = TableNameMapper (type);
            } else {
#if NETSTANDARD1_3
                var info = type.GetTypeInfo ();
#else
                var info = type;
#endif
                //NOTE: This as dynamic trick falls back to handle both our own Table-attribute as well as the one in EntityFramework
                var tableAttrName = info.GetAttribute<Schema.TableAttribute> (false)?.Name ?? (info.GetCustomAttributes (false)
                    .FirstOrDefault (attr => attr.GetType ().Name == "TableAttribute") as dynamic)?.Name;

                if (tableAttrName != null) {
                    name = tableAttrName;
                } else {
                    name = type.Name + "s";
                    if (type.IsInterface && name.StartsWith ("I")) {
                        name = name.Substring (1);
                    }
                }
            }

            TypeTableName[type.TypeHandle] = name;
            return name;
        }

        /// <summary>
        /// ��ȡƥ�����
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="paramName">������</param>
        /// <param name="matchType">ƥ������</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetMatchExpression<T> (this IDbConnection connection, string paramName, MatchType matchType) {
            var adapter = GetFormatter (connection);
            var column = GetColumnName<T> (paramName);
            var matchExp = adapter.GetColumnMatchesValue (column, paramName, matchType);
            return matchExp;
        }

        private static class ProxyGenerator {
            private static readonly Dictionary<Type, Type> TypeCache = new Dictionary<Type, Type> ();

            private static AssemblyBuilder GetAsmBuilder (string name) {
#if NETSTANDARD1_3 || NETSTANDARD2_0
                return AssemblyBuilder.DefineDynamicAssembly (new AssemblyName { Name = name }, AssemblyBuilderAccess.Run);
#else
                return Thread.GetDomain ().DefineDynamicAssembly (new AssemblyName { Name = name }, AssemblyBuilderAccess.Run);
#endif
            }

            public static T GetInterfaceProxy<T> () {
                Type typeOfT = typeof (T);

                if (TypeCache.TryGetValue (typeOfT, out Type k)) {
                    return (T) k.CreateInstance ();
                }

                var assemblyBuilder = GetAsmBuilder (typeOfT.Name);
                var moduleBuilder = assemblyBuilder.DefineDynamicModule ("SqlMapperExtensions." + typeOfT.Name); //NOTE: to save, add "asdasd.dll" parameter

                var interfaceType = typeof (IProxy);
                var typeBuilder = moduleBuilder.DefineType (typeOfT.Name + "_" + Guid.NewGuid (),
                    TypeAttributes.Public | TypeAttributes.Class);

                typeBuilder.AddInterfaceImplementation (typeOfT);
                typeBuilder.AddInterfaceImplementation (interfaceType);

                //create our _isDirty field, which implements IProxy
                var setIsDirtyMethod = CreateIsDirtyProperty (typeBuilder);

                // Generate a field for each property, which implements the T
                foreach (var property in typeof (T).GetProperties ()) {
                    var isId = property.GetCustomAttributes (true).Any (a => a is KeyAttribute);
                    CreateProperty<T> (typeBuilder, property.Name, property.PropertyType, setIsDirtyMethod, isId);
                }

#if NETSTANDARD1_3 || NETSTANDARD2_0
                var generatedType = typeBuilder.CreateTypeInfo ().AsType ();
#else
                var generatedType = typeBuilder.CreateType ();
#endif

                TypeCache.Add (typeOfT, generatedType);
                return (T) generatedType.CreateInstance ();
            }

            private static MethodInfo CreateIsDirtyProperty (TypeBuilder typeBuilder) {
                var propType = typeof (bool);
                var field = typeBuilder.DefineField ("_" + nameof (IProxy.IsDirty), propType, FieldAttributes.Private);
                var property = typeBuilder.DefineProperty (nameof (IProxy.IsDirty),
                    System.Reflection.PropertyAttributes.None,
                    propType,
                    new [] { propType });

                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.SpecialName |
                    MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                var currGetPropMthdBldr = typeBuilder.DefineMethod ("get_" + nameof (IProxy.IsDirty),
                    getSetAttr,
                    propType,
                    Type.EmptyTypes);
                var currGetIl = currGetPropMthdBldr.GetILGenerator ();
                currGetIl.Emit (OpCodes.Ldarg_0);
                currGetIl.Emit (OpCodes.Ldfld, field);
                currGetIl.Emit (OpCodes.Ret);
                var currSetPropMthdBldr = typeBuilder.DefineMethod ("set_" + nameof (IProxy.IsDirty),
                    getSetAttr,
                    null,
                    new [] { propType });
                var currSetIl = currSetPropMthdBldr.GetILGenerator ();
                currSetIl.Emit (OpCodes.Ldarg_0);
                currSetIl.Emit (OpCodes.Ldarg_1);
                currSetIl.Emit (OpCodes.Stfld, field);
                currSetIl.Emit (OpCodes.Ret);

                property.SetGetMethod (currGetPropMthdBldr);
                property.SetSetMethod (currSetPropMthdBldr);
                var getMethod = typeof (IProxy).GetMethod ("get_" + nameof (IProxy.IsDirty));
                var setMethod = typeof (IProxy).GetMethod ("set_" + nameof (IProxy.IsDirty));
                typeBuilder.DefineMethodOverride (currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride (currSetPropMthdBldr, setMethod);

                return currSetPropMthdBldr;
            }

            private static void CreateProperty<T> (TypeBuilder typeBuilder, string propertyName, Type propType, MethodInfo setIsDirtyMethod, bool isIdentity) {
                //Define the field and the property
                var field = typeBuilder.DefineField ("_" + propertyName, propType, FieldAttributes.Private);
                var property = typeBuilder.DefineProperty (propertyName,
                    System.Reflection.PropertyAttributes.None,
                    propType,
                    new [] { propType });

                const MethodAttributes getSetAttr = MethodAttributes.Public |
                    MethodAttributes.Virtual |
                    MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                var currGetPropMthdBldr = typeBuilder.DefineMethod ("get_" + propertyName,
                    getSetAttr,
                    propType,
                    Type.EmptyTypes);

                var currGetIl = currGetPropMthdBldr.GetILGenerator ();
                currGetIl.Emit (OpCodes.Ldarg_0);
                currGetIl.Emit (OpCodes.Ldfld, field);
                currGetIl.Emit (OpCodes.Ret);

                var currSetPropMthdBldr = typeBuilder.DefineMethod ("set_" + propertyName,
                    getSetAttr,
                    null,
                    new [] { propType });

                //store value in private field and set the isdirty flag
                var currSetIl = currSetPropMthdBldr.GetILGenerator ();
                currSetIl.Emit (OpCodes.Ldarg_0);
                currSetIl.Emit (OpCodes.Ldarg_1);
                currSetIl.Emit (OpCodes.Stfld, field);
                currSetIl.Emit (OpCodes.Ldarg_0);
                currSetIl.Emit (OpCodes.Ldc_I4_1);
                currSetIl.Emit (OpCodes.Call, setIsDirtyMethod);
                currSetIl.Emit (OpCodes.Ret);

                //TODO: Should copy all attributes defined by the interface?
                if (isIdentity) {
                    var keyAttribute = typeof (KeyAttribute);
                    var myConstructorInfo = keyAttribute.GetConstructor (new Type[] { });
                    var attributeBuilder = new CustomAttributeBuilder (myConstructorInfo, new object[] { });
                    property.SetCustomAttribute (attributeBuilder);
                }

                property.SetGetMethod (currGetPropMthdBldr);
                property.SetSetMethod (currSetPropMthdBldr);
                var getMethod = typeof (T).GetMethod ("get_" + propertyName);
                var setMethod = typeof (T).GetMethod ("set_" + propertyName);
                typeBuilder.DefineMethodOverride (currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride (currSetPropMthdBldr, setMethod);
            }
        }
    }
}