#pragma warning disable CS0618

using System;
using System.Collections.Generic;
using System.Linq;

namespace CcNetCore.Utils.Extensions {
    /// <summary>
    /// 时间精度枚举
    /// </summary>
    public enum TimePrecision {
        Day = 0,
        Hour,
        Minute,
        Second,
        Millisecond,
    }

    /// <summary>
    /// DateTime扩展类
    /// </summary>
    public static class DateTimeExtension {
        /// <summary>
        ///
        /// </summary>
        private static DateTime Date_1970_1_1 = new DateTime (1970, 1, 1, 0, 0, 0);

        /// <summary>
        /// 每天的小时数
        /// </summary>
        public const int HOURS_OF_DAY = 24;

        /// <summary>
        /// 每小时的分钟数
        /// </summary>
        public const int MINUTES_OF_HOUR = 60;

        /// <summary>
        /// 每分钟的秒数
        /// </summary>
        public const int SECONDS_OF_MINUTE = 60;

        /// <summary>
        /// 每秒的毫秒数
        /// </summary>
        public const int MILLISECONDS_OF_SECOND = 1000;

        /// <summary>
        /// 获取UNIX时间戳（秒级别）
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp () {
            return GetTimeStamp (DateTime.Now).ToString ();
        }

        /// <summary>
        /// 获取UNIX时间戳（秒级别）
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimeStamp (this DateTime dateTime) {
            var diff = dateTime - Date_1970_1_1;
            return Convert.ToInt64 (diff.TotalSeconds);
        }

        /// <summary>
        /// 将UNIX时间戳（秒级别）转换为具体的时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime FromTimeStamp (long timeStamp) {
            var dateTime = Date_1970_1_1.AddSeconds (timeStamp);
            return dateTime;
        }

        /// <summary>
        /// 转换成Unix时间
        /// </summary>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        public static long GetUnixTime (this DateTime nowTime) {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime (Date_1970_1_1);
            var time = (long) Math.Round ((nowTime - startTime).TotalSeconds, MidpointRounding.AwayFromZero);
            return time;
        }

        /// <summary>
        /// 解析Unix时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime (long timeStamp) {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime (Date_1970_1_1);
            var time = startTime.AddSeconds (timeStamp).ToLocalTime ();
            return time;
        }

        /// <summary>
        /// 格式化输出日期
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetString (this DateTime? dt) {
            if (null == dt) {
                return string.Empty;
            }

            return dt.Value.ToString ();
        }

        /// <summary>
        /// 格式化输出日期
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format">输出格式</param>
        /// <returns></returns>
        public static string GetString (this DateTime? dt, string format) {
            if (null == dt) {
                return string.Empty;
            }

            return dt.Value.ToString (format);
        }

        /// <summary>
        /// 获取一天的起始时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ToDayStart (this DateTime dt) =>
            new DateTime (dt.Year, dt.Month, dt.Day, 0, 0, 0);

        /// <summary>
        /// 获取一天的结束时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ToDayEnd (this DateTime dt) =>
            new DateTime (dt.Year, dt.Month, dt.Day, 23, 59, 59);

        /// <summary>
        /// 获取绝对时间差
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="other">另一个时间</param>
        /// <returns></returns>
        public static TimeSpan GetAbsDistance (this DateTime dt, DateTime other) {
            if (dt > other) {
                return dt - other;
            } else {
                return other - dt;
            }
        }

        /// <summary>
        /// 获取显示时间
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="precision">时间精度</param>
        /// <param name="includeDay">是否显示天数</param>
        /// <returns></returns>
        public static string GetTimeText (this long ticks, TimePrecision precision, bool includeDay = true) {
            return TimeSpan.FromTicks (ticks).GetTimeText (precision, includeDay);
        }

        /// <summary>
        /// 获取显示时间
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="precision">时间精度</param>
        /// <param name="includeDay">是否显示天数</param>
        /// <returns></returns>
        public static string GetTimeText (this TimeSpan ts, TimePrecision precision, bool includeDay = true) {
            var totalMilliseconds = ts.TotalMilliseconds;
            if (totalMilliseconds <= 0) {
                return string.Empty;
            }

            var text = new List<string> ();

            var totalSeconds = (int) totalMilliseconds / MILLISECONDS_OF_SECOND;
            var remainMilliseconds = totalMilliseconds - totalSeconds * MILLISECONDS_OF_SECOND;

            var totalMinutes = (int) totalSeconds / SECONDS_OF_MINUTE;
            var remainSeconds = totalSeconds - totalMinutes * SECONDS_OF_MINUTE;

            if (precision >= TimePrecision.Second) {
                if (remainMilliseconds > 0) {
                    text.Add (string.Format ("{0}.{1}秒", remainSeconds, remainMilliseconds));
                } else if (remainSeconds > 0) {
                    text.Add (string.Format ("{0}秒", remainSeconds));
                }
            }

            var totalHours = (int) totalMinutes / MINUTES_OF_HOUR;
            var remainMinutes = totalMinutes - totalHours * MINUTES_OF_HOUR;

            if (precision >= TimePrecision.Minute) {
                if (remainMinutes > 0) {
                    text.Add (string.Format ("{0}分", remainMinutes));
                }
            }

            var totalDays = (int) totalHours / HOURS_OF_DAY;
            var remainHours = totalHours - totalDays * HOURS_OF_DAY;

            if (includeDay) {
                if (precision >= TimePrecision.Hour) {
                    if (remainHours > 0) {
                        text.Add (string.Format ("{0}小时", remainHours));
                    }
                }

                if (precision >= TimePrecision.Day) {
                    if (totalDays > 0) {
                        text.Add (string.Format ("{0}天", totalDays));
                    }
                }
            } else {
                if (precision >= TimePrecision.Hour) {
                    if (totalHours > 0) {
                        text.Add (string.Format ("{0}小时", totalHours));
                    }
                }
            }

            if (text.Any ()) {
                text.Reverse ();
            }

            return string.Join (string.Empty, text);
        }
    }
}