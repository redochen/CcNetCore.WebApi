using System;
using System.ComponentModel;
using System.Linq;

namespace CcNetCore.Utils.Extensions {
    /// <summary>
    /// Enum扩展类
    /// </summary>
    public static class EnumExtension {
        /// <summary>
        /// 获取枚举类型的描述
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static string GetDesc (this Enum enumeration) {
            Type type = enumeration.GetType ();
            var members = type.GetMember (enumeration.ToString ());
            if (!members.IsEmpty ()) {
                var attrs = members.First ().GetCustomAttributes (typeof (DescriptionAttribute), false);
                if (!attrs.IsEmpty ()) {
                    return ((DescriptionAttribute) attrs.First ()).Description;
                }
            }

            return enumeration.ToString ();
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="traveller"></param>
        public static void ForEach<TEnum> (Action<TEnum /*value*/> traveller) {
            if (null == traveller) {
                return;
            }

            var values = Enum.GetValues (typeof (TEnum));
            if (values.IsEmptyEx ()) {
                return;
            }

            foreach (TEnum current in values) {
                traveller (current);
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="traveller"></param>
        public static void ForEach<TEnum> (Action<int /*index*/ , TEnum /*value*/> traveller) {
            if (null == traveller) {
                return;
            }

            var values = Enum.GetValues (typeof (TEnum));
            if (values.IsEmptyEx ()) {
                return;
            }

            var index = 0;
            foreach (TEnum current in values) {
                traveller (index, current);
                index++;
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="traveller">traveller返值：True=break；False=continue</param>
        public static void ForEach<TEnum> (Func<TEnum /*value*/ , bool /*break*/> traveller) {
            if (null == traveller) {
                return;
            }

            var values = Enum.GetValues (typeof (TEnum));
            if (values.IsEmptyEx ()) {
                return;
            }

            foreach (TEnum current in values) {
                if (traveller (current)) {
                    break;
                }
            }
        }

        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="traveller">traveller返值：True=break；False=continue</param>
        public static void ForEach<TEnum> (Func<int /*index*/ , TEnum /*value*/ , bool /*break*/> traveller) {
            if (null == traveller) {
                return;
            }

            var values = Enum.GetValues (typeof (TEnum));
            if (values.IsEmptyEx ()) {
                return;
            }

            var index = 0;
            foreach (TEnum current in values) {
                if (traveller (index, current)) {
                    break;
                }

                index++;
            }
        }
    }
}