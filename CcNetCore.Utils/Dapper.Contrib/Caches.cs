using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using CcNetCore.Utils.Extensions;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// The Dapper.Contrib extensions for Dapper
    /// </summary>
    public static partial class SqlMapperExtensions {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ExplicitKeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ColumnNameProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> AutoIncrementProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> IgnoredProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> RequiredPorperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ();
        private static readonly ConcurrentDictionary < RuntimeTypeHandle, IEnumerable < (PropertyInfo Property, TypeConverterAttribute Attribute) >> ConverterProperties = new ConcurrentDictionary < RuntimeTypeHandle, IEnumerable < (PropertyInfo Property, TypeConverterAttribute Attribute) >> ();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetQueries = new ConcurrentDictionary<RuntimeTypeHandle, string> ();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string> ();
        private static readonly ConcurrentDictionary<TypeConverterAttribute, TypeConverter> TypeConverters = new ConcurrentDictionary<TypeConverterAttribute, TypeConverter> ();
        private static readonly ConcurrentDictionary<PropertyInfo, ColumnAttribute> PropertyColumns = new ConcurrentDictionary<PropertyInfo, ColumnAttribute> ();
        private static List<PropertyInfo> ColumnNameProperitiesCache (Type type) {
            if (ColumnNameProperties.TryGetValue (type.TypeHandle, out IEnumerable<PropertyInfo> pi)) {
                return pi.ToList ();
            }

            var columnNameProperities = GetAttributeProperties<ColumnAttribute> (type);

            ColumnNameProperties[type.TypeHandle] = columnNameProperities;
            return columnNameProperities;
        }

        private static List<PropertyInfo> AutoIncrementPropertiesCache (Type type) {
            if (AutoIncrementProperties.TryGetValue (type.TypeHandle, out IEnumerable<PropertyInfo> pi)) {
                return pi.ToList ();
            }

            var autoIncrementAttribute = GetAttributeProperties<AutoIncrementAttribute> (type);

            AutoIncrementProperties[type.TypeHandle] = autoIncrementAttribute;
            return autoIncrementAttribute;
        }

        private static List<PropertyInfo> IgnoredPropertiesCache (Type type) {
            if (IgnoredProperties.TryGetValue (type.TypeHandle, out IEnumerable<PropertyInfo> pi)) {
                return pi.ToList ();
            }

            var ignoredProperties = GetAttributeProperties<IgnoreAttribute> (type);

            IgnoredProperties[type.TypeHandle] = ignoredProperties;
            return ignoredProperties;
        }

        private static List<PropertyInfo> RequiredPorpertiesCache (Type type) {
            if (RequiredPorperties.TryGetValue (type.TypeHandle, out IEnumerable<PropertyInfo> pi)) {
                return pi.ToList ();
            }

            var requiredProperties = GetAttributeProperties<RequiredAttribute> (type);
            //TypePropertiesCache(type).Where(p => p.GetCustomAttributes(true).Any(a => a is RequiredAttribute)).ToList();

            RequiredPorperties[type.TypeHandle] = requiredProperties;
            return requiredProperties;
        }

        private static List<PropertyInfo> ExplicitKeyPropertiesCache (Type type) {
            if (ExplicitKeyProperties.TryGetValue (type.TypeHandle, out IEnumerable<PropertyInfo> pi)) {
                return pi.ToList ();
            }

            var explicitKeyProperties = GetAttributeProperties<ExplicitKeyAttribute> (type);
            //TypePropertiesCache(type).Where(p => p.GetCustomAttributes(true).Any(a => a is ExplicitKeyAttribute)).ToList();

            ExplicitKeyProperties[type.TypeHandle] = explicitKeyProperties;
            return explicitKeyProperties;
        }

        private static List<PropertyInfo> KeyPropertiesCache (Type type) {
            if (KeyProperties.TryGetValue (type.TypeHandle, out IEnumerable<PropertyInfo> pi)) {
                return pi.ToList ();
            }

            var allProperties = TypePropertiesCache (type);
            var keyProperties = GetAttributeProperties<KeyAttribute> (type);
            //allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

            if (keyProperties.Count == 0) {
                var idProp = allProperties.Find (p => string.Equals (p.Name, "id", StringComparison.CurrentCultureIgnoreCase));
                if (idProp != null && !idProp.GetCustomAttributes (true).Any (a => a is ExplicitKeyAttribute)) {
                    keyProperties.Add (idProp);
                }
            }

            KeyProperties[type.TypeHandle] = keyProperties;
            return keyProperties;
        }

        private static List < (PropertyInfo Property, TypeConverterAttribute Attribute) > ConverterPropertiesCache (Type type) {
            if (ConverterProperties.TryGetValue (type.TypeHandle, out IEnumerable < (PropertyInfo Property, TypeConverterAttribute Attribute) > pis)) {
                return pis.ToList ();
            }

            var propertyConverters = GetAttributeProperties<TypeConverterAttribute> (type)?.Select < PropertyInfo,
                (PropertyInfo, TypeConverterAttribute) > (
                    p => (p, p.GetAttribute<TypeConverterAttribute> (inherit: true)));
            ConverterProperties[type.TypeHandle] = propertyConverters;

            return propertyConverters.ToList ();
        }

        private static List<PropertyInfo> TypePropertiesCache (Type type) {
            if (TypeProperties.TryGetValue (type.TypeHandle, out IEnumerable<PropertyInfo> pis)) {
                return pis.ToList ();
            }

            var properties = type.GetProperties ().Where (IsWriteable).ToArray ();
            TypeProperties[type.TypeHandle] = properties;
            return properties.ToList ();
        }

        private static ColumnAttribute PropertyColumnsCache (PropertyInfo property) {
            //var handle = property.PropertyType.TypeHandle;
            if (PropertyColumns.TryGetValue (property, out ColumnAttribute attr)) {
                return attr;
            }

            attr = ColumnNameProperitiesCache (property.ReflectedType) ?
                .Where (p => p.Name.Equals (property.Name)) ?
                .Select (p => p.GetAttribute<ColumnAttribute> (true)).FirstOrDefault ();
            PropertyColumns[property] = attr;
            return attr;
        }
    }
}