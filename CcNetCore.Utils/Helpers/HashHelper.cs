using System.IO;
using System.Security.Cryptography;
using System.Text;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.Utils.Helpers {
    /// <summary>
    /// Hash帮助类
    /// </summary>
    public sealed class HashHelper {
        #region 计算MD5
        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ComputeMD5 (string text) => ComputeMD5 (text.ToBytes ());

        /// <summary>
        /// 计算数据的MD5值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ComputeMD5 (byte[] data) {
            if (data.IsEmpty ()) {
                return string.Empty;
            }

            using (var md5 = MD5.Create ()) {
                return ComputeHash (md5, data);
            }
        }

        /// <summary>
        /// 计算流的MD5值
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ComputeMD5 (Stream stream) {
            if (null == stream) {
                return string.Empty;
            }

            using (var md5 = MD5.Create ()) {
                return ComputeHash (md5, stream);
            }
        }
        #endregion

        #region 计算CRC32
        /// <summary>
        /// 计算字符串的CRC32值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ComputeCRC32 (string text) => ComputeMD5 (text.ToBytes ());

        /// <summary>
        /// 计算数据的CRC32值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ComputeCRC32 (byte[] data) {
            if (data.IsEmpty ()) {
                return string.Empty;
            }

            using (var crc32 = CRC32.Create ()) {
                return ComputeHash (crc32, data);
            }
        }

        /// <summary>
        /// 计算流的CRC32值
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ComputeCRC32 (Stream stream) {
            if (null == stream) {
                return string.Empty;
            }

            using (var crc32 = CRC32.Create ()) {
                return ComputeHash (crc32, stream);
            }
        }
        #endregion

        #region 计算SHA1
        /// <summary>
        /// 计算字符串的SHA1值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ComputeSHA1 (string text) => ComputeMD5 (text.ToBytes ());
        /// <summary>
        /// 计算数据的SHA1值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ComputeSHA1 (byte[] data) {
            if (data.IsEmpty ()) {
                return string.Empty;
            }

            using (var sha1 = SHA1.Create ()) {
                return ComputeHash (sha1, data);
            }
        }

        /// <summary>
        /// 计算流的SHA1值
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ComputeSHA1 (Stream stream) {
            if (null == stream) {
                return string.Empty;
            }

            using (var sha1 = SHA1.Create ()) {
                return ComputeHash (sha1, stream);
            }
        }
        #endregion

        /// <summary>
        /// 计算指定数据的Hash值
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string ComputeHash (HashAlgorithm algorithm, byte[] data) {
            if (null == algorithm || data.IsEmpty ()) {
                return string.Empty;
            }

            return GetHexString (algorithm.ComputeHash (data));
        }

        /// <summary>
        /// 计算指定数据的Hash值
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static string ComputeHash (HashAlgorithm algorithm, Stream stream) {
            if (null == algorithm || null == stream) {
                return string.Empty;
            }

            return GetHexString (algorithm.ComputeHash (stream));
        }

        /// <summary>
        /// 获取十六进制的字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetHexString (byte[] data) {
            if (!data.IsValid ()) {
                return string.Empty;
            }

            //将字节数组转换成十六进制的字符串形式
            var sb = new StringBuilder ();

            for (int i = 0; i < data.Length; i++) {
                sb.Append (data[i].ToString ("x2"));
            }

            return sb.ToString ();
        }
    }
}