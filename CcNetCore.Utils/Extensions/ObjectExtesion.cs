#pragma warning disable CS0168

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CcNetCore.Utils.Attributes;
using Newtonsoft.Json;

namespace CcNetCore.Utils.Extensions {
    /// <summary>
    /// Object扩展类
    /// </summary>
    public static class ObjectExtension {
        private readonly static Regex RgxDouble = new Regex (@"^\d+\.0+$");

        /// <summary>
        /// 是否有效
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="flags">检测标识</param>
        /// <returns></returns>
        public static bool IsValid (this object obj, CheckValidFlag flags = CheckValidFlag.Default) {
            if (null == obj) {
                return false;
            }

            if (DBNull.Value.Equals (obj)) {
                return flags.Contains (CheckValidFlag.DbNullAsValid);
            }

            var type = obj.GetType ();

            if (type.IsValueType) {
                return ((obj.TryDouble () == 0 || obj.TryDouble () == 0.0F) ? flags.Contains (CheckValidFlag.ZeroAsValid) : true);
            } else if (type == typeof (string)) {
                return (obj as string).IsValid (!flags.Contains (CheckValidFlag.WhiteSpaceAsValid));
            } else {
                return true;
            }
        }

        /// <summary>
        /// 判断一个对象的值是否等于该对象类型的默认值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDefault (this object obj) {
            if (null == obj) {
                return true;
            }

            var type = obj.GetType (retriveUnderlyingType: true);
            if (typeof (string) == type) {
                return obj.ToString () == string.Empty;
            } else if (type.IsValueType) {
                return obj.Equals (type.CreateInstance ());
            }

            return false;
        }

        /// <summary>
        /// 判断是否为集合类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCollection (this Type type) {
            if (null == type) {
                return false;
            }

            if (type.IsArray) {
                return true;
            }

            return (typeof (ICollection) == type || typeof (ICollection<>) == type);
        }

        /// <summary>
        /// 判断是否为值类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stringAsValueType">是否将String类型视为值类型</param>
        /// <returns></returns>
        public static bool IsValueType (this Type type, bool stringAsValueType = true) {
            if (type.IsValueType) {
                return true;
            }

            if (stringAsValueType && type == typeof (string)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 将对象转换成特定的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public static T ChangeType<T> (this object obj, out string error) {
            error = null;

            try {
                var value = obj.ChangeType (typeof (T), out error);
                if (value != null) {
                    return (T) value;
                }
            } catch (Exception ex) {
                error = ex.Message;
            }

            return default (T);
        }

        /// <summary>
        /// 将对象转换成特定的类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="conversionType">要返回的对象的类型</param>
        /// <param name="error">错误信息</param>
        /// <param name="converter">类型转换实例</param>
        /// <returns></returns>
        public static object ChangeType (this object obj, Type conversionType,
            out string error, TypeConverter converter = null) {
            error = null;

            try {
                if (null == conversionType) {
                    error = "要返回的对象的类型不能为Null";
                    return null;
                }

                if (null == obj) {
                    return conversionType.CreateInstance ();
                }

                if (obj.GetType () == conversionType) {
                    return obj;
                }

                var str = obj.ToString ();
                if (!str.IsValid ()) {
                    return conversionType.CreateInstance ();
                }

                if (converter != null) {
                    return converter.ConvertFrom (str);
                } else {
                    converter = TypeDescriptor.GetConverter (conversionType);
                    return converter.ConvertFromString (str);
                }
            } catch (Exception ex) {
                error = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取对象的类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="retriveUnderlyingType">是否返回可空类型的根类型</param>
        /// <returns></returns>
        public static Type GetType (this object obj, bool retriveUnderlyingType) {
            if (null == obj) {
                return null;
            }

            switch (obj) {
                case Type type:
                    return type;
                case MemberInfo member:
                    return member.GetMemberType (retriveUnderlyingType: retriveUnderlyingType);
                default:
                    return obj.GetType ();
            }
        }

        /// <summary>
        /// 将对象转换成Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreNull">是否忽略Null值</param>
        /// <returns></returns>
        public static string ToJson (this object obj, bool ignoreNull = true) {
            try {
            if (obj != null) {
            if (ignoreNull) {
            var settings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore
                        };

                        return JsonConvert.SerializeObject (obj, settings);
                    } else {
                        return JsonConvert.SerializeObject (obj);
                    }
                }
            } catch (Exception ex) { }

            return string.Empty;
        }

        #region 验证方法组
        /// <summary>
        /// 根据特定的模型来验证对象的值是否有效
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        /// <param name="validation_type">验证类型</param>
        /// <param name="order_by">验证顺序参数</param>
        /// <param name="invalid_member">验证不通过的属性或字段</param>
        /// <returns>错误信息</returns>
        public static string Validate<TModel> (this object obj, ValidationType validation_type,
            List<string> order_by, out string invalid_member)
        where TModel : class, new () {
            invalid_member = null;

            if (null == obj) {
                return string.Empty;
            }

            var model = new TModel ();
            var members = model.GetFieldsAndProperities (inherit: true, includeStatic: false);
            if (members.IsEmpty ()) {
                return string.Empty;
            }

            List<MemberInfo> sortedMembers = null;
            if (order_by.IsEmpty ()) {
                sortedMembers = members.ToList ();
            } else {
                sortedMembers = new List<MemberInfo> ();

                foreach (var order in order_by) {
                    var member = members.FirstOrDefault (x => x.Name.Equals (order));
                    if (member != null) {
                        sortedMembers.Add (member);
                    }
                }

                foreach (var m in members) {
                    if (!sortedMembers.Contains (m)) {
                        sortedMembers.Add (m);
                    }
                }
            }

            foreach (var member in sortedMembers) {
                var error = model.Validate (member, obj.GetValue (member.Name), validation_type);
                if (error.IsValid ()) {
                    invalid_member = member.Name;
                    return error;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 验证对象某个属性或字段的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">属性或字段的名称</param>
        /// <param name="value">要验证的值/param>
        /// <param name="type">验证类型</param>
        /// <returns>错误信息</returns>
        public static string Validate (this object obj, string name, object value, ValidationType type) {
            if (null == obj || !name.IsValid ()) {
                return string.Empty;
            }

            var member = obj.GetFieldOrProperity (name, inherit : true, includeStatic : false);
            if (null == member) {
                return string.Empty;
            }

            return obj.Validate (member, value, type);
        }

        /// <summary>
        /// 验证对象某个属性或字段的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="member">属性或字段</param>
        /// <param name="value">要验证的值/param>
        /// <param name="type">验证类型</param>
        /// <returns>错误信息</returns>
        public static string Validate (this object obj, MemberInfo member, object value, ValidationType type) {
            if (null == obj || null == member) {
                return string.Empty;
            }

            string error = null;

            //数字验证
            error = member.Validate<NumberValidationAttribute> (value, type);
            if (error.IsValid ()) {
                return error;
            }

            //字符号验证
            error = member.Validate<StringValidationAttribute> (value, type);
            if (error.IsValid ()) {
                return error;
            }

            //必填验证
            error = member.Validate<RequiredAttribute> (value, type);
            if (error.IsValid ()) {
                return error;
            }

            return string.Empty;
        }

        /// <summary>
        /// 使用特定的验证器验证对象的值
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member">属性或字段</param>
        /// <param name="value">要验证的值</param>
        /// <param name="type">验证类型</param>
        /// <returns>错误信息</returns>
        private static string Validate<TAttribute> (this MemberInfo member, object value, ValidationType type)
        where TAttribute : ValidationAttribute {
            if (null == member) {
                return string.Empty;
            }

            var attributes = member.GetAttributes<TAttribute> (inherit: false);
            if (attributes.IsEmpty ()) {
                return string.Empty;
            }

            foreach (var attr in attributes) {
                var bva = attr as BaseValidationAttribute;
                if (bva != null && !bva.Type.Contains (type)) {
                    continue;
                }

                if (!attr.IsValid (value)) {
                    return attr.ErrorMessage;
                }
            }

            return string.Empty;
        }
        #endregion

        /// <summary>
        /// 获取对象属性的描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name">字段或属性名称</param>
        /// <returns></returns>
        public static string GetDesc<T> (this T obj, string name) {
            var descs = obj.GetDescs (inherit: true);
            if (null == descs || !descs.ContainsKey (name)) {
                return string.Empty;
            }

            return descs[name];
        }

        /// <summary>
        /// 获取对象的描述列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDescs<T> (this T obj, bool inherit) {
            try {
                if (null == obj) {
                    return null;
                }

                var members = obj.GetFieldsAndProperities (inherit: inherit, includeStatic: true);
                if (members.IsEmpty ()) {
                    return null;
                }

                var dict = new Dictionary<string, string> ();
                foreach (var member in members) {
                    var attr = member.GetAttribute<DescriptionAttribute> (inherit);
                    if (attr != null) {
                        dict.Add (member.Name, attr.Description);
                    }
                }

                return dict;
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 转换成特定的实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ToSpecObject<T> (this object obj) {
            if (null == obj) {
                return default (T);
            }

            try {
                switch (obj) {
                    case DataRow dr:
                        return dr.ToObject<T> ();
                    case DataRowView drv:
                        return drv.Row.ToObject<T> ();
                    case Hashtable ht:
                        return ht.ToObject<T> (inherit: true);
                    default:
                        return obj.ChangeType<T> (out string error);
                }
            } catch (Exception ex) {
                return default (T);
            }
        }

        #region 类型转换
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToStringEx (this object obj) {
            try {
                if (null == obj) {
                    return string.Empty;
                }

                return obj.ToString ();
            } catch (Exception ex) {
                return string.Empty;
            }
        }

        /// <summary>
        /// 将小数转换成字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="accuracy">精度</param>
        /// <returns></returns>
        public static string ToString (this decimal value, int accuracy = 2) {
            return ((double) value).ToString (accuracy);
        }

        /// <summary>
        /// 将小数转换成字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="accuracy">精度</param>
        /// <returns></returns>
        public static string ToString (this double value, int accuracy = 2) {
            if (accuracy < 0) {
                accuracy = 0;
            }

            var str = value.ToString ($"F{accuracy}");
            if (!RgxDouble.IsMatch (str)) {
                return str;
            }

            return value.ToString ("F0");
        }

        /// <summary>
        /// 转换成布尔型数值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ToBool (this object obj, bool defaultValue = false) => obj.TryBool () ?? defaultValue;

        /// <summary>
        /// 尝试转换成布尔型数值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool? TryBool (this object obj) {
            try {
                return Convert.ToBoolean (obj);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 转换成Int型数值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt (this object obj, int defaultValue = 0) => obj.TryInt () ?? defaultValue;

        /// <summary>
        /// 尝试转换成Int型数值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? TryInt (this object obj) {
            var str = obj.GetNumberString ();
            if (str.IsValid ()) {
                return (int) Convert.ToDouble (str);
            }

            return null;
        }

        /// <summary>
        /// 转换成Long型数值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToLong (this object obj, long defaultValue = 0L) => obj.TryLong () ?? defaultValue;

        /// <summary>
        /// 尝试转换成Long型数值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long? TryLong (this object obj) {
            var str = obj.GetNumberString ();
            if (str.IsValid ()) {
                return (long) Convert.ToDouble (str);
            }

            return null;
        }

        /// <summary>
        /// 转换成Decimal型数值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal (this object obj, decimal defaultValue = 0.0M) => obj.TryDecimal () ?? defaultValue;

        /// <summary>
        /// 尝试转换成Decimal型数值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal? TryDecimal (this object obj) {
            var str = obj.GetNumberString ();
            if (str.IsValid ()) {
                return decimal.Parse (str, NumberStyles.Float);
            }

            return null;
        }

        /// <summary>
        /// 转换成Double型数值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble (this object obj, double defaultValue = 0.0F) => obj.TryDouble () ?? defaultValue;

        /// <summary>
        /// 尝试转换成Double型数值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double? TryDouble (this object obj) {
            var str = obj.GetNumberString ();
            if (str.IsValid ()) {
                return double.Parse (str, NumberStyles.Float);
            }

            return null;
        }

        /// <summary>
        /// 获取数值字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetNumberString (this object obj) {
            if (null == obj || DBNull.Value == obj) {
                return string.Empty;
            }

            try {
                var str = obj.ToString ();

                if (str.UpperCase ().Contains ("E")) {
                    return decimal.Parse (str, NumberStyles.Float).ToString ("G");
                }

                if (Regex.IsMatch (str, RegExpPatterns.IntegerNumbers) ||
                    Regex.IsMatch (str, RegExpPatterns.FloatNumbers)) {
                    return str;
                }
            } catch (Exception ex) { }

            return string.Empty;
        }
        #endregion

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或属性的名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool SetValue (this object obj, string name, object value) {
            if (null == obj || !name.IsValid ()) {
                return false;
            }

            switch (obj) {
                case DataRow dr:
                    dr.SetValue (name, value, false);
                    break;
                case DataRowView drv:
                    drv.Row.SetValue (name, value, false);
                    break;
                case Hashtable ht:
                    return ht.SetValue<string, object> (name, value);
                default:
                    return obj.SetMemberValue (name, value, inherit : true,
                        includeStatic : false, createDefaultInstanceForAllNullObject : false);
            }

            return true;
        }

        /// <summary>
        /// 设置字段或属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或属性的名称（支持model.member风格的名称）</param>
        /// <param name="value">值</param>
        /// <param name="pathChar">路径字符</param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="includeStatic">是否包含静态的成员</param>
        /// <param name="createDefaultInstanceForAllNullObject">为路径上的所有空对象创建默认值</param>
        /// <returns></returns>
        public static bool SetMemberValue (this object obj, string name, object value, char pathChar = '.',
            bool inherit = true, bool includeStatic = false, bool createDefaultInstanceForAllNullObject = false) {
            try {
                if (null == obj || !name.IsValid ()) {
                    return false;
                }

                var member = obj.GetMember (name, out object parentObj, inherit : inherit,
                    includeStatic : includeStatic, includeFields : true, includeProperities : true,
                    includeMethods : false, includeEvents : false, includeTypeInfo : false, pathChar : pathChar,
                    createDefaultInstanceForAllNullObject : createDefaultInstanceForAllNullObject);

                if (null == parentObj) {
                    return false;
                }

                return parentObj.SetMemberValue (member, value);
            } catch (Exception ex) {
                return false;
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或属性的名称</param>
        /// <returns></returns>
        public static object GetValue (this object obj, string name) {
            if (null == obj || !name.IsValid ()) {
                return null;
            }

            switch (obj) {
                case DataRow dr:
                    return dr.GetValue (name);
                case DataRowView drv:
                    return drv.Row.GetValue (name);
                case Hashtable ht:
                    return ht.GetValue<string, object> (name);
                default:
                    return obj.GetMemberValue (name, inherit : true, includeStatic : false,
                        createDefaultInstanceForAllNullObject : false);
            }
        }

        /// <summary>
        /// 获取字段或属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或属性的名称（支持model.member风格的名称）</param>
        /// <param name="pathChar">路径字符</param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="includeStatic">是否包含静态的成员</param>
        /// <param name="createDefaultInstanceForAllNullObject">为路径上的所有空对象创建默认值</param>
        /// <returns></returns>
        public static object GetMemberValue (this object obj, string name, char pathChar = '.',
            bool inherit = true, bool includeStatic = true, bool createDefaultInstanceForAllNullObject = false) {
            try {
                if (null == obj || !name.IsValid ()) {
                    return null;
                }

                var member = obj.GetMember (name, out object parentObj, inherit : inherit,
                    includeStatic : includeStatic, includeFields : true, includeProperities : true,
                    includeMethods : false, includeEvents : false, includeTypeInfo : false, pathChar : pathChar,
                    createDefaultInstanceForAllNullObject : createDefaultInstanceForAllNullObject);

                return member?.GetMemberValue (parentObj);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 将数据转换成字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encodingName">编码名称</param>
        /// <returns></returns>
        public static string GetString (this byte[] data, string encodingName = "UTF-8") {
            if (data.IsEmpty ()) {
                return string.Empty;
            }

            return Encoding.GetEncoding (encodingName).GetString (data);
        }

        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或变量名称</param>
        /// <returns></returns>
        public static string GetString (this object obj, string name) =>
            obj?.GetValue (name)?.ToString ();

        /// <summary>
        /// 获取整数值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或变量名称</param>
        /// <returns></returns>
        public static int? GetInt (this object obj, string name) {
            var value = obj.GetString (name);
            if (!value.IsValid ()) {
                return null;
            }

            try {
                return Convert.ToInt32 (value);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 获取长整数值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或变量名称</param>
        /// <returns></returns>
        public static long? GetLong (this object obj, string name) {
            var value = obj.GetString (name);
            if (!value.IsValid ()) {
                return null;
            }

            try {
                return Convert.ToInt64 (value);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 获取实数数值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或变量名称</param>
        /// <returns></returns>
        public static decimal? GetDecimal (this object obj, string name) {
            var value = obj.GetString (name);
            if (!value.IsValid ()) {
                return null;
            }

            try {
                return Convert.ToDecimal (value);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 获取双精度数值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或变量名称</param>
        /// <returns></returns>
        public static double? GetDouble (this object obj, string name) {
            var value = obj.GetString (name);
            if (!value.IsValid ()) {
                return null;
            }

            try {
                return Convert.ToDouble (value);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 获取布尔值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或变量名称</param>
        /// <returns></returns>
        public static bool? GetBool (this object obj, string name) {
            var value = obj.GetValue (name);
            if (null == value) {
                return null;
            }

            try {
                return Convert.ToBoolean (value);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 获取日期时间值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或变量名称</param>
        /// <returns></returns>
        public static DateTime? GetDateTime (this object obj, string name) {
            var value = obj.GetValue (name);
            if (null == value) {
                return null;
            }

            try {
                if (value is long l) {
                    var str = l.ToString ();
                    if (str.Length > 10) {
                        str = str.Substring (0, 10);
                    }

                    return DateTimeExtension.FromTimeStamp (str.ToLong ());
                }

                return Convert.ToDateTime (value);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 复制对象的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="includeStatic">是否包含静态的成员</param>
        /// <returns></returns>
        public static T CopyValue<T> (this T self, T other, bool inherit = false, bool includeStatic = false)
        where T : class, new () => self.CopyValueEx (other, inherit : inherit, includeStatic : includeStatic);

        /// <summary>
        /// 复制对象的值
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="includeStatic">是否包含静态的成员</param>
        /// <returns></returns>
        public static T1 CopyValueEx<T1, T2> (this T1 self, T2 other, bool inherit, bool includeStatic)
        where T1 : class, new ()
        where T2 : class {
            if (null == other) {
                return self;
            }

            if (null == self) {
                self = Activator.CreateInstance<T1> ();
            }

            var members = other.GetFieldsAndProperities (inherit: inherit, includeStatic: includeStatic);
            foreach (var member in members) {
                var value = member.GetMemberValue (other);
                self.SetValue (member.Name, value);
            }

            return self;
        }

        /// <summary>
        /// 比较对象的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <returns>TRUE：两个实例的值都相同；FALSE：两个实例的值不同</returns>
        public static bool CompareValue<T> (this T self, T other, bool inherit)
        where T : class, new () {
            if (null == self || null == other) {
                return self == other;
            }

            var members = self.GetFieldsAndProperities (inherit: inherit, includeStatic: false);
            foreach (var member in members) {
                var selfValue = member.GetMemberValue (self);
                var otherValue = member.GetMemberValue (other);

                if (!selfValue.EqualsEx (otherValue, true)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 比较两个对象是否相等
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="strongCompare">是否强比较（类型必须相同）</param>
        /// <returns></returns>
        public static bool EqualsEx (this object self, object other, bool strongCompare = true) {
            var selfNull = (null == self);
            var otherNull = (null == other);

            //两者只有一个为null时
            if (selfNull ^ otherNull) {
                return false;
            }

            //两者都为null，或都不为null
            if (self == other || other.Equals (self)) {
                return true;
            }

            var selfType = self.GetType ();
            var otherType = other.GetType ();

            if (selfType == otherType) {
                if (selfType.IsValueType) {
                    return false;
                } else if (selfType == typeof (string)) {
                    return CompareStringValue (self, other);
                } else //引用类型调用CompareValue比较
                {
                    return self.CompareValue (other, inherit : true);
                }
            }

            //强比较模板下，类型不相同直接返回False
            if (strongCompare) {
                return false;
            }

            var isSelfValueType = selfType.IsValueType (true);
            var isOtherValueType = otherType.IsValueType (true);

            //两者只有一个为值类型时，或者都不为值类型时
            if ((isSelfValueType ^ isOtherValueType) || !isSelfValueType) {
                return false;
            }

            return CompareStringValue (self, other);

            #region 比较字符串值
            bool CompareStringValue (object obj1, object obj2) {
                var str1 = obj1.ToString ().GetValue ();
                var str2 = obj2.ToString ().GetValue ();

                return str1.Equals (str2);
            }
            #endregion
        }

        /// <summary>
        /// 从Hashtable中解析数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ht"></param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <returns></returns>
        public static T ToObject<T> (this Hashtable ht, bool inherit) {
            var t = Activator.CreateInstance<T> ();
            var members = t.GetFieldsAndProperities (inherit: inherit, includeStatic: true);
            foreach (var member in members) {
                if (!ht.ContainsKey (member.Name)) {
                    continue;
                }

                t.SetMemberValue (member, ht[member.Name]);
            }

            return t;
        }

        /// <summary>
        /// 将对象转换成Hashtable
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="removeNull">是否移除空值属性或字段</param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <returns></returns>
        public static Hashtable ToHashtable (this object obj, bool removeNull, bool inherit) {
            if (null == obj) {
                return null;
            }

            var members = obj.GetFieldsAndProperities (inherit: inherit, includeStatic: true);
            if (members.IsEmpty ()) {
                return null;
            }

            var hash = new Hashtable ();

            foreach (var member in members) {
                var value = member.GetMemberValue (obj);
                var valObj = value.GetValueObject (removeNull, inherit);

                if (valObj != null || !removeNull) {
                    hash.Add (member.Name, valObj);
                }
            }

            return hash;
        }

        /// <summary>
        /// 获取值对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="removeNull">是否移除空值属性或字段</param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <returns></returns>
        private static object GetValueObject (this object obj, bool removeNull, bool inherit) {
            if (null == obj) {
                return null;
            }

            switch (obj) {
                case string str:
                    return obj;
                case DateTime dt:
                    return dt.ToString ("yyyy-MM-dd HH:mm:ss");
                case Array array:
                    {
                        var items = new List<object> ();
                        foreach (var item in array) {
                            var val = item.GetValueObject (removeNull, inherit);
                            if (val != null) {
                                items.Add (val);
                            }
                        }
                        return items.Count > 0 ? items : null;
                    }
                case IList list:
                    {
                        var items = new List<object> ();
                        foreach (var item in list) {
                            var val = item.GetValueObject (removeNull, inherit);
                            if (val != null) {
                                items.Add (val);
                            }
                        }
                        return items.Count > 0 ? items : null;
                    }
                case IDictionary dic:
                    {
                        var ht = new Hashtable ();
                        foreach (DictionaryEntry de in dic) {
                            var val = de.Value.GetValueObject (removeNull, inherit);
                            if (val != null) {
                                ht.Add (de.Key, val);
                            }
                        }

                        return ht.Count > 0 ? ht : null;
                    }
                default:
                    if (obj.GetType ().IsValueType) {
                        return obj;
                    }

                    return obj.ToHashtable (removeNull, inherit);
            }
        }

        /// <summary>
        /// 设置对象的成员值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="member">成员对象</param>
        /// <param name="value">成员的值</param>
        /// <returns>成功或失败</returns>
        public static bool SetMemberValue (this object obj, MemberInfo member, object value) {
            if (null == obj || null == member) {
                return false;
            }

            try {
                var type = member.GetMemberType (out Type underlyingType);
                if (null == type) {
                    return false;
                }

                if (type.IsArray) //数组类型,单独处理
                {
                    return member.SetMemberValue (obj, value);
                }

                if (null == value) {
                    if (typeof (string) == type || type.IsNullableType () || !type.IsValueType) {
                        return member.SetMemberValue (obj, null);
                    }
                }

                //转换值的类型
                value = value.ChangeType (underlyingType, out string error);

                if (type.IsValueType (stringAsValueType: true)) //值类型
                {
                    return member.SetMemberValue (obj, value);
                }

                var valueTemp = value.Clone (out bool handled);
                if (!handled) {
                    valueTemp = underlyingType.CreateInstance ();
                    valueTemp.CopyValue (value, true, false);
                }

                return member.SetMemberValue (obj, valueTemp);
            } catch (Exception ex) {
                return false;
            }
        }

        /// <summary>
        /// 调用非泛型方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="method">方法名称</param>
        /// <returns></returns>
        public static object Invoke (this object obj, string method) => obj.Invoke (method, parameters : null, inherit : true, types : null);

        /// <summary>
        /// 调用非泛型方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="method">方法名称</param>
        /// <param name="parameters">参数集合</param>
        /// <returns></returns>
        public static object Invoke (this object obj, string method, object[] parameters) => obj.Invoke (method, parameters : parameters, inherit : true, types : null);

        /// <summary>
        /// 调用非泛型方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="method">方法名称</param>
        /// <param name="parameters">参数集合</param>
        /// <param name="types">参数类型集合</param>
        /// <returns></returns>
        public static object Invoke (this object obj, string method, object[] parameters, Type[] types) => obj.Invoke (method, parameters : parameters, inherit : true, types : types);

        /// <summary>
        /// 调用非泛型方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="method">方法名称</param>
        /// <param name="parameters">参数集合</param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="types">参数类型集合</param>
        /// <returns></returns>
        private static object Invoke (this object obj, string method, object[] parameters, bool inherit, Type[] types) {
            var methodInfo = obj.GetMethodInfo (method, inherit : inherit, types : types);
            if (null == methodInfo) {
                return null;
            }

            return methodInfo.Invoke (obj, parameters);
        }

        /// <summary>
        /// 调用泛型方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="method">方法名称</param>
        /// <param name="parameters">参数集合</param>
        /// <param name="genericTypes">泛型参数集合</param>
        /// <returns></returns>
        public static object InvokeGeneric (this object obj, string method, object[] parameters, params Type[] genericTypes) => obj.InvokeGeneric (method, parameters : parameters, inherit : true, types : null, genericTypes : genericTypes);

        /// <summary>
        /// 调用泛型方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="method">方法名称</param>
        /// <param name="parameters">参数集合</param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="types">参数类型集合</param>
        /// <param name="genericTypes">泛型参数集合</param>
        /// <returns></returns>
        private static object InvokeGeneric (this object obj, string method, object[] parameters, bool inherit, Type[] types, params Type[] genericTypes) {
            var methodInfo = obj.GetMethodInfo (method, inherit : inherit, types : types);
            if (null == methodInfo) {
                return null;
            }

            var genericMethod = methodInfo.MakeGenericMethod (genericTypes);
            if (null == genericMethod) {
                return null;
            }

            return genericMethod.Invoke (obj, parameters);
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="method">方法名称</param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="types">参数类型集合</param>
        /// <returns></returns>
        private static MethodInfo GetMethodInfo (this object obj, string method, bool inherit, Type[] types) {
            if (null == obj) {
                return null;
            }

            if (!(obj is Type type)) //obj可以为Type,也可以为对象实例
            {
                type = obj.GetType ();
            }

            if (null == type) {
                return null;
            }

            MethodInfo methodInfo = null;

            var bindingFlags = GetBindingFlags (inherit: inherit, includeStatic: true);
            if (types.IsEmpty ()) {
                methodInfo = type.GetMethod (method, bindingFlags);
            } else {
                methodInfo = type.GetMethod (method, bindingFlags, Type.DefaultBinder, types,
                    new ParameterModifier[] { new ParameterModifier (types.Length) });
            }

            return methodInfo;
        }

        /// <summary>
        /// 获取事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventName">事件名称</param>
        /// <returns></returns>
        public static EventInfo GetEventInfo (this object obj, string eventName) {
            if (null == obj) {
                return null;
            }

            var type = obj.GetType ();
            if (null == type) {
                return null;
            }

            var bindingFlags = GetBindingFlags (inherit: false, includeStatic: false);
            return type.GetEvent (eventName);
        }

        /// <summary>
        /// 判断对象是否含有指定名称的字段或属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或属性的名称（支持model.member风格的名称）</param>
        /// <param name="pathChar">路径字符</param>
        /// <returns></returns>
        public static bool HasFieldOrProperity (this object obj, string name, char pathChar = '.') => obj.GetFieldOrProperity (name, inherit : true, includeStatic : true, pathChar : pathChar) != null;

        /// <summary>
        /// 获取对象的特定字段或属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">字段或属性的名称（支持model.member风格的名称）</param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="includeStatic">是否包含静态的成员</param>
        /// <param name="pathChar">路径字符</param>
        /// <returns></returns>
        public static MemberInfo GetFieldOrProperity (this object obj, string name,
            bool inherit, bool includeStatic, char pathChar = '.') => obj.GetMember (name, out object parentObj, inherit : inherit, includeStatic : includeStatic,
            includeFields : true, includeProperities : true, includeMethods : false, includeEvents : false,
            includeTypeInfo : false, pathChar : pathChar, createDefaultInstanceForAllNullObject : false);

        /// <summary>
        /// 获取对象的字段和属性列表
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="includeStatic">是否包含静态的成员</param>
        /// <returns></returns>
        public static IEnumerable<MemberInfo> GetFieldsAndProperities (this object obj, bool inherit, bool includeStatic) => obj.GetMembers (inherit: inherit, includeStatic: includeStatic,
            includeFields: true, includeProperities: true, includeMethods: false,
            includeEvents: false, includeTypeInfo: false);

        /// <summary>
        /// 成员信息缓存
        /// </summary>
        private static ConcurrentDictionary<string, IEnumerable<MemberInfo>> MembersCache = new ConcurrentDictionary<string, IEnumerable<MemberInfo>> ();

        /// <summary>
        /// 获取对象的特定成员
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">成员名称（支持model.member风格的名称）</param>
        /// <param name="parentObject">最终属性的父级对象实例</param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="includeStatic">是否包含静态的成员</param>
        /// <param name="includeFields">是否包含字段</param>
        /// <param name="includeProperities">是否包含属性</param>
        /// <param name="includeMethods">是否包含方法</param>
        /// <param name="includeEvents">是否包含事件</param>
        /// <param name="includeTypeInfo">是否包含类型信息</param>
        /// <param name="pathChar">路径字符</param>
        /// <param name="createDefaultInstanceForAllNullObject">为路径上的所有空对象创建默认值</param>
        /// <returns></returns>
        private static MemberInfo GetMember (this object obj, string name, out object parentObject,
            bool inherit, bool includeStatic, bool includeFields, bool includeProperities,
            bool includeMethods, bool includeEvents, bool includeTypeInfo,
            char pathChar = '.', bool createDefaultInstanceForAllNullObject = false) {
            parentObject = null;

            try {
                if (null == obj) {
                    return null;
                }

                var type = obj.GetType (retriveUnderlyingType: true);
                if (null == type) {
                    return null;
                }

                var flags = GetBindingFlags (inherit: inherit, includeStatic: includeStatic);
                var types = GetMemberTypes (includeFields: includeFields,
                    includeProperities: includeProperities,
                    includeMethods: includeMethods,
                    includeEvents: includeEvents,
                    includeTypeInfo: includeTypeInfo);

                MemberInfo member = null;
                Type tempType = type, tempParentType = type;
                object tempObj = null, tempParentObj = null;

                //防止从类型中取值（应从实例中取值）
                tempObj = tempParentObj = (obj is Type) ? null : obj;

                name.Split (pathChar).ForEach (x => {
                    var members = tempType.GetMember (x, flags);
                    member = members?.FirstOrDefault (y => (types & y.MemberType) == y.MemberType);
                    if (null == member) {
                        return true; //break
                    }

                    tempParentType = tempType;
                    tempParentObj = tempObj;

                    tempType = member.GetMemberType (retriveUnderlyingType: true);
                    if (tempObj != null) {
                        tempObj = member.GetMemberValue (tempObj);
                    }

                    if (!createDefaultInstanceForAllNullObject) {
                        return false; //continue
                    }

                    if (null == tempParentObj) {
                        tempParentObj = tempParentType.CreateInstance ();
                    }

                    if (null == tempObj) {
                        tempObj = tempType.CreateInstance ();
                        member.SetMemberValue (tempParentObj, tempObj);
                    }

                    return false; //continue
                });

                parentObject = tempParentObj;

                return member;
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 获取对象的成员列表
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="includeStatic">是否包含静态的成员</param>
        /// <param name="includeFields">是否包含字段</param>
        /// <param name="includeProperities">是否包含属性</param>
        /// <param name="includeMethods">是否包含方法</param>
        /// <param name="includeEvents">是否包含事件</param>
        /// <param name="includeTypeInfo">是否包含类型信息</param>
        /// <returns></returns>
        public static IEnumerable<MemberInfo> GetMembers (this object obj, bool inherit,
            bool includeStatic, bool includeFields, bool includeProperities,
            bool includeMethods, bool includeEvents, bool includeTypeInfo) {
            try {
                if (null == obj) {
                    return null;
                }

                var type = obj.GetType (retriveUnderlyingType: true);
                if (null == type) {
                    return null;
                }

                var flags = GetBindingFlags (inherit: inherit, includeStatic: includeStatic);
                var types = GetMemberTypes (includeFields: includeFields,
                    includeProperities: includeProperities,
                    includeMethods: includeMethods,
                    includeEvents: includeEvents,
                    includeTypeInfo: includeTypeInfo);

                var cacheKey = $"{type.FullName} [{flags}]";

                if (!MembersCache.TryGetValue (cacheKey, out IEnumerable<MemberInfo> members)) {
                    //过滤掉<XXX>k__BackingField
                    members = type.GetMembers (flags)?.Where (x => !x.Name.IsBackField ());

                    MembersCache.AddOrUpdate (cacheKey, members, (k, v) => {
                        v = members;
                        return members;
                    });
                }

                return members?.Where (x => (types & x.MemberType) == x.MemberType);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 获取对象的成员值
        /// </summary>
        /// <param name="member"></param>
        /// <param name="obj">对象实例</param>
        /// <returns></returns>
        public static object GetMemberValue (this MemberInfo member, object obj) {
            try {
                if (null == obj || null == member) {
                    return null;
                }

                if (member is PropertyInfo pi) {
                    return pi.GetValue (obj, null);
                }

                if (member is FieldInfo fi) {
                    return fi.GetValue (obj);
                }
            } catch (Exception ex) { }

            return null;
        }

        /// <summary>
        /// 设置对象的属性或字段的值
        /// </summary>
        /// <param name="member"></param>
        /// <param name="obj">对象实例</param>
        /// <param name="value">成员的值</param>
        /// <returns></returns>
        public static bool SetMemberValue (this MemberInfo member, object obj, object value) {
            try {
                if (null == obj || null == member) {
                    return false;
                }

                if (member is PropertyInfo pi) {
                    pi.SetValue (obj, value, null);
                    return true;
                }

                if (member is FieldInfo fi) {
                    fi.SetValue (obj, value);
                    return true;
                }
            } catch (Exception ex) { }

            return false;
        }

        /// <summary>
        /// 获取成员的类型
        /// </summary>
        /// <param name="member"></param>
        /// <param name="retriveUnderlyingType">是否返回可空类型的根类型</param>
        /// <returns></returns>
        public static Type GetMemberType (this MemberInfo member, bool retriveUnderlyingType = false) {
            var type = member.GetMemberType (out Type underlyingType);
            return retriveUnderlyingType ? underlyingType : type;
        }

        /// <summary>
        /// 获取成员的类型
        /// </summary>
        /// <param name="member"></param>
        /// <param name="underlyingType">根类型</param>
        /// <returns></returns>
        public static Type GetMemberType (this MemberInfo member, out Type underlyingType) {
            underlyingType = null;

            if (null == member) {
                return null;
            }

            Type type = null;

            switch (member) {
                case PropertyInfo pi:
                    type = pi.PropertyType;
                    break;
                case FieldInfo fi:
                    type = fi.FieldType;
                    break;
                default:
                    type = member.GetType ();
                    break;
            }

            if (type.IsNullableType ()) {
                underlyingType = type.GetUnderlyingType ();
            } else {
                underlyingType = type;
            }

            return type;
        }

        /// <summary>
        /// 判断是否为可空类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType (this Type type) {
            if (null == type) {
                return false;
            }

            return (type.IsGenericType && typeof (Nullable<>) == type.GetGenericTypeDefinition ());
        }

        /// <summary>
        /// 获取可空类型的根类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetUnderlyingType (this Type type) {
            if (type.IsNullableType ()) {
                return Nullable.GetUnderlyingType (type);
            }

            return type;
        }

        /// <summary>
        /// 获取成员的特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="member">成员实例</param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute> (this MemberInfo member, bool inherit)
        where TAttribute : Attribute => member.GetAttributes<TAttribute> (inherit)?.FirstOrDefault ();

        /// <summary>
        /// 获取成员的特性集合
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="member">成员实例</param>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <returns></returns>
        public static TAttribute[] GetAttributes<TAttribute> (this MemberInfo member, bool inherit)
        where TAttribute : Attribute {
            var attrType = typeof (TAttribute);
            if (null == member || !member.IsDefined (attrType, inherit)) {
                return null;
            }

            var attrs = Attribute.GetCustomAttributes (member, attrType);
            return attrs as TAttribute[];
        }

        /// <summary>
        /// 获取绑定标识（注意：子类是无法调用继承类的私有成员）
        /// </summary>
        /// <param name="inherit">是否搜索继承链中的成员</param>
        /// <param name="includeStatic">是否包含静态的成员</param>
        /// <returns></returns>
        private static BindingFlags GetBindingFlags (bool inherit, bool includeStatic) {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            if (includeStatic) {
                flags |= BindingFlags.Static;
            }

            if (!inherit) {
                flags |= BindingFlags.DeclaredOnly;

                if (includeStatic) {
                    flags |= BindingFlags.FlattenHierarchy;
                }
            }

            return flags;
        }

        /// <summary>
        /// 获取成员类型
        /// </summary>
        /// <param name="includeFields">是否包含字段</param>
        /// <param name="includeProperities">是否包含属性</param>
        /// <param name="includeMethods">是否包含方法</param>
        /// <param name="includeEvents">是否包含事件</param>
        /// <param name="includeTypeInfo">是否包含类型信息</param>
        /// <returns></returns>
        private static MemberTypes GetMemberTypes (bool includeFields, bool includeProperities,
            bool includeMethods, bool includeEvents, bool includeTypeInfo) {
            MemberTypes types = 0;

            if (includeFields) {
                types |= MemberTypes.Field;
            }

            if (includeProperities) {
                types |= MemberTypes.Property;
            }

            if (includeMethods) {
                types |= MemberTypes.Method;
            }

            if (includeEvents) {
                types |= MemberTypes.Event;
            }

            if (includeTypeInfo) {
                types |= MemberTypes.TypeInfo;
            }

            return types;
        }
    }

    /// <summary>
    /// 实例扩展类
    /// </summary>
    public static class InstanceExtension {
        /// <summary>
        /// 使用默认构造函数创建实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInstance<T> () {
            try {
                var obj = (typeof (T)).CreateInstance (out bool handled);
                if (handled) {
                    return (T) obj;
                }

                return Activator.CreateInstance<T> ();
            } catch (Exception ex) {
                return default (T);
            }
        }

        /// <summary>
        /// 使用默认构造函数创建实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstance (this Type type) {
            try {
                var obj = type.CreateInstance (out bool handled);
                if (handled) {
                    return obj;
                }

                return Activator.CreateInstance (type);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 使用带参数的构造函数创建实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args">构造函数参数</param>
        /// <returns></returns>
        public static object CreateInstance (this Type type, params object[] args) {
            try {
                return Activator.CreateInstance (type, args);
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// 使用默认构造函数创建实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handled">是否已处理</param>
        /// <returns></returns>
        private static object CreateInstance (this Type type, out bool handled) {
            handled = false;

            try {
                if (null == type) {
                    return null;
                }

                if (SpecialTypeCreaters.ContainsKey (type)) {
                    handled = true;
                    return SpecialTypeCreaters[type]?.Invoke (type);
                }

                if (SpecialTypeNameCreaters.ContainsKey (type.FullName)) {
                    handled = true;
                    return SpecialTypeNameCreaters[type.FullName]?.Invoke ();
                }
            } catch (Exception ex) { }

            return null;
        }

        /// <summary>
        /// 特殊类型的实例创建器
        /// </summary>
        private static readonly Dictionary<string, Func<object>> SpecialTypeNameCreaters = new Dictionary<string, Func<object>> {
            ["System.String"] = () => string.Empty,
            //["System.Drawing.Font"] = () => SystemFonts.DefaultFont.Clone (),
            //["System.Drawing.FontFamily"] = () => new FontFamily (SystemFonts.DefaultFont.FontFamily.Name),
        };

        /// <summary>
        /// 特殊类型的实例创建器
        /// </summary>
        private static readonly Dictionary<Type, Func<Type, object>> SpecialTypeCreaters = new Dictionary<Type, Func<Type, object>> {

        };

        /// <summary>
        /// 浅表复制方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T CloneEx<T> (this T obj) {
            try {
                var shadow = obj.Clone (out bool handled);
                if (handled) {
                    return (T) shadow;
                }

                return (T) obj.Invoke ("MemberwiseClone");
            } catch (Exception ex) {
                return default (T);
            }
        }

        /// <summary>
        /// 对象复制
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        public static object Clone (this object obj, out bool handled) {
            handled = false;

            try {
                if (null == obj) {
                    return null;
                }

                var type = obj.GetType ();

                if (SpecialTypeCloner.ContainsKey (type)) {
                    handled = true;
                    return SpecialTypeCloner[type]?.Invoke (obj);
                }

                if (SpecialTypeNameCloner.ContainsKey (type.FullName)) {
                    handled = true;
                    return SpecialTypeNameCloner[type.FullName]?.Invoke (obj);
                }
            } catch (Exception ex) { }

            return null;
        }

        /// <summary>
        /// 特殊类型的实例复制器
        /// </summary>
        private static readonly Dictionary<string, Func<object, object>> SpecialTypeNameCloner = new Dictionary<string, Func<object, object>> {
            //["System.Drawing.Font"] = (obj) => ((Font) obj).Clone (),
        };

        /// <summary>
        /// 特殊类型的实例复制器
        /// </summary>
        private static readonly Dictionary<Type, Func<object, object>> SpecialTypeCloner = new Dictionary<Type, Func<object, object>> {

        };
    }
}