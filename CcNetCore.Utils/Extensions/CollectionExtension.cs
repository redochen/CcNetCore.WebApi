#pragma warning disable CS0168

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CcNetCore.Utils.Extensions {
    /// <summary>
    /// 集合扩展类
    /// </summary>
    public static class CollectionExtension {
        /// <summary>
        /// 判断集合是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool IsEmpty<T> (this IEnumerable<T> enumerable) => (null == enumerable || !enumerable.Any ());

        /// <summary>
        /// 判断集合是否为空
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsEmptyEx (this ICollection collection) => (null == collection || collection.Count <= 0);

        #region ForEach方法组
        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="traveller"></param>
        public static void ForEach<T> (this IEnumerable<T> enumerable, Action<T /*value*/> traveller) {
            if (enumerable.IsEmpty () || null == traveller) {
                return;
            }

            foreach (var current in enumerable) {
                traveller (current);
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="traveller"></param>
        public static void ForEach<T> (this IEnumerable<T> enumerable, Action<int /*index*/ , T /*value*/> traveller) {
            if (enumerable.IsEmpty () || null == traveller) {
                return;
            }

            var index = 0;
            foreach (var current in enumerable) {
                traveller (index, current);
                index++;
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="traveller">traveller返值：True=break；False=continue</param>
        public static void ForEach<T> (this IEnumerable<T> enumerable, Func<T /*value*/ , bool /*break*/> traveller) {
            if (enumerable.IsEmpty () || null == traveller) {
                return;
            }

            foreach (var current in enumerable) {
                if (traveller (current)) {
                    break;
                }
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="traveller">traveller返值：True=break；False=continue</param>
        public static void ForEach<T> (this IEnumerable<T> enumerable, Func<int /*index*/ , T /*value*/ , bool /*break*/> traveller) {
            if (enumerable.IsEmpty () || null == traveller) {
                return;
            }

            var index = 0;
            foreach (var current in enumerable) {
                if (traveller (index, current)) {
                    break;
                }

                index++;
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="traveller"></param>
        public static void ForEach<T> (this ICollection collection, Action<T /*value*/> traveller) {
            if (collection.IsEmptyEx () || null == traveller) {
                return;
            }

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
        public static void ForEach<T> (this ICollection collection, Action<int /*index*/ , T /*value*/> traveller) {
            if (collection.IsEmptyEx () || null == traveller) {
                return;
            }

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
        /// <param name="traveller">traveller返值：True=break；False=continue</param>
        public static void ForEach<T> (this ICollection collection, Func<T /*value*/ , bool /*break*/> traveller) {
            if (collection.IsEmptyEx () || null == traveller) {
                return;
            }

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
        /// <param name="traveller">traveller返值：True=break；False=continue</param>
        public static void ForEach<T> (this ICollection collection, Func<int /*index*/ , T /*value*/ , bool /*break*/> traveller) {
            if (collection.IsEmptyEx () || null == traveller) {
                return;
            }

            var index = 0;
            foreach (T current in collection) {
                if (traveller (index, current)) {
                    break;
                }

                index++;
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="traveller"></param>
        public static void ForEach<TKey, TValue> (this IDictionary<TKey, TValue> dictionary, Action<TKey /*key*/ , TValue /*value*/> traveller) {
            if (dictionary.IsEmpty () || null == traveller) {
                return;
            }

            foreach (var current in dictionary) {
                traveller (current.Key, current.Value);
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="traveller"></param>
        public static void ForEach<TKey, TValue> (this IDictionary<TKey, TValue> dictionary,
            Action<int /*index*/ , TKey /*key*/ , TValue /*value*/> traveller) {
            if (dictionary.IsEmpty () || null == traveller) {
                return;
            }

            var index = 0;
            foreach (var current in dictionary) {
                traveller (index, current.Key, current.Value);
                index++;
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="traveller">traveller返值：True=break；False=continue</param>
        public static void ForEach<TKey, TValue> (this IDictionary<TKey, TValue> dictionary,
            Func<TKey /*key*/ , TValue /*value*/ , bool /*break*/> traveller) {
            if (dictionary.IsEmpty () || null == traveller) {
                return;
            }

            foreach (var current in dictionary) {
                if (traveller (current.Key, current.Value)) {
                    break;
                }
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="traveller">traveller返值：True=break；False=continue</param>
        public static void ForEach<TKey, TValue> (this IDictionary<TKey, TValue> dictionary,
            Func<int /*index*/ , TKey /*key*/ , TValue /*value*/ , bool /*break*/> traveller) {
            if (dictionary.IsEmpty () || null == traveller) {
                return;
            }

            var index = 0;
            foreach (var current in dictionary) {
                if (traveller (index, current.Key, current.Value)) {
                    break;
                }

                index++;
            }
        }
        #endregion

        /// <summary>
        /// 判断集合是否包含特定项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool ContainsEx<T> (this IEnumerable<T> enumerable, T item) => (!enumerable.IsEmpty () && enumerable.Contains (item));

        /// <summary>
        /// 判断字典中是否包含特定键
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsKeyEx<TKey, TValue> (this IDictionary<TKey, TValue> dictionary, TKey key) => (dictionary != null && dictionary.ContainsKey (key));

        /// <summary>
        /// 将集合中的符合条件（或全部）的项投影到另一个集合中
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDest">目标类型</typeparam>
        /// <param name="enumerable"></param>
        /// <param name="selector">选择方法</param>
        /// <param name="predicate">过滤方法</param>
        /// <returns></returns>
        public static List<TDest> SelectEx<TSource, TDest> (this IEnumerable<TSource> enumerable,
            Func<TSource, TDest> selector, Func<TSource, bool> predicate = null) {
            if (enumerable.IsEmpty ()) {
                return null;
            }

            var list = new List<TDest> ();

            foreach (var item in enumerable) {
                if (predicate?.Invoke (item) ?? true) {
                    list.Add (selector (item));
                }
            }

            return list;
        }

        /// <summary>
        /// 将集合中的符合条件（或全部）的项投影到另一个集合中
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="collection"></param>
        /// <param name="selector">选择方法</param>
        /// <param name="predicate">过滤方法</param>
        /// <returns></returns>
        public static List<TDest> SelectEx<TSource, TDest> (this ICollection collection,
            Func<TSource, TDest> selector, Func<TSource, bool> predicate = null) {
            if (collection.IsEmptyEx () || null == selector) {
                return null;
            }

            var list = new List<TDest> ();

            collection.ForEach<TSource> (item => {
                if (predicate?.Invoke (item) ?? true) {
                    list.Add (selector (item));
                }
            });

            return list;
        }

        /// <summary>
        /// 基于谓词筛选值序列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate">过滤方法</param>
        /// <returns></returns>
        public static List<T> Where<T> (this ICollection collection, Func<T, bool> predicate) {
            if (collection.IsEmptyEx () || null == predicate) {
                return null;
            }

            var list = new List<T> ();

            collection.ForEach<T> (item => {
                if (predicate (item)) {
                    list.Add (item);
                }
            });

            return list;
        }

        /// <summary>
        /// 返回序列中的第一个元素；如果序列中不包含任何元素，则返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate">过滤方法</param>
        /// <returns></returns>
        public static T FirstOrDefault<T> (this ICollection collection, Func<T, bool> predicate = null) {
            T result = default (T);

            if (collection.IsEmptyEx ()) {
                return result;
            }

            collection.ForEach<T> (item => {
                var success = (predicate?.Invoke (item) ?? true);
                if (success) {
                    result = item;
                }

                return success;
            });

            return result;
        }

        /// <summary>
        /// 返回序列中的最后一个元素；如果序列中不包含任何元素，则返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate">过滤方法</param>
        /// <returns></returns>
        public static T LastOrDefault<T> (this ICollection collection, Func<T, bool> predicate = null) {
            T result = default (T);

            if (collection.IsEmptyEx ()) {
                return result;
            }

            collection.ForEach<T> (item => {
                if ((predicate?.Invoke (item) ?? true)) {
                    result = item;
                }
            });

            return result;
        }

        /// <summary>
        /// 返回序列中指定索引处的元素；如果索引超出范围，则返回默认值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <param name="index">要检索的从零开始的元素索引</param>
        /// <returns></returns>
        public static T ElementAtOrDefault<T> (this ICollection collection, int index) {
            T result = default (T);

            if (collection.IsEmptyEx ()) {
                return result;
            }

            if (index < 0 || index >= collection.Count) {
                return result;
            }

            collection.ForEach<T> ((i, item) => {
                if (i == index) {
                    result = item;
                    return true;
                }

                return false;
            });

            return result;
        }

        /// <summary>
        /// 设置值（存在时覆盖，不存在时添加）
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetValue<TKey, TValue> (this IDictionary dictionary, TKey key, TValue value) {
            if (null == dictionary || null == key) {
                return false;
            }

            if (dictionary.Contains (key)) {
                dictionary[key] = value;
            } else {
                dictionary.Add (key, value);
            }

            return true;
        }

        /// <summary>
        /// 批量设置值（存在时覆盖，不存在时添加）
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool SetValues<TKey, TValue> (this IDictionary dictionary, IDictionary other) {
            if (null == dictionary || null == other) {
                return false;
            }

            if (other.Count <= 0) {
                return false;
            }

            foreach (DictionaryEntry de in other) {
                dictionary.SetValue (de.Key, de.Value);
            }

            return true;
        }

        /// <summary>
        /// 从字典中移除键值对
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void RemoveKey<TKey> (this IDictionary dictionary, TKey key) {
            if (null == dictionary || null == key) {
                return;
            }

            if (dictionary.Contains (key)) {
                dictionary.Remove (key);
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue> (this IDictionary dictionary, TKey key) {
            if (null == dictionary || null == key) {
                return default (TValue);
            }

            if (!dictionary.Contains (key) || null == dictionary[key]) {
                return default (TValue);
            }

            var value = dictionary[key];
            var type = typeof (TValue);
            if (value.GetType () == type) {
                return (TValue) value;
            }

            try {
                return (TValue) Convert.ChangeType (value, type);
            } catch (Exception ex) {
                return (TValue) value;
            }
        }

        /// <summary>
        /// 字典的Key-Value互换
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static Dictionary<TValue, TKey> Exchange<TKey, TValue> (this IDictionary<TKey, TValue> dictionary) {
            if (dictionary.IsEmpty ()) {
                return null;
            }

            var dicResult = new Dictionary<TValue, TKey> ();

            foreach (var kvp in dictionary) {
                if (kvp.Value != null && !dicResult.ContainsKey (kvp.Value)) {
                    dicResult.Add (kvp.Value, kvp.Key);
                }
            }

            return dicResult;
        }
    }
}