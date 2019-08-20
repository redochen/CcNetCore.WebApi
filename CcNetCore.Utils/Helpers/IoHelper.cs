using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CcNetCore.Utils.Extensions;

namespace CcNetCore.Utils.Helpers {
    /// <summary>
    /// IO帮助类
    /// </summary>
    public static class IoHelper {
        /// <summary>
        /// 判断指定的路径是否为文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public static bool IsFile (string path, out string error) {
            error = null;

            try {
                if (!path.IsValid ()) {
                    return false;
                }

                return File.Exists (path);
            } catch (Exception ex) {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 判断指定的路径是否为文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public static bool IsDirectory (string path, out string error) {
            error = null;

            try {
                if (!path.IsValid ()) {
                    return false;
                }

                return Directory.Exists (path);
            } catch (Exception ex) {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="error">错误信息</param>
        /// <param name="searchSubFolderFiles">是否查找子目录的文件</param>
        /// <returns></returns>
        public static List<FileInfo> GetFiles (string path, out string error, bool searchSubFolderFiles = false) {
            List<FileInfo> files = null;
            List<DirectoryInfo> folders = null;

            error = GetFilesAndFolders (path, ref files, ref folders,
                searchFiles : true, searchSubFolderFiles : searchSubFolderFiles,
                searchFolders : false, searchSubFolders : false);

            return files;
        }

        /// <summary>
        /// 获取所有目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="error">错误信息</param>
        /// <param name="searchSubFolders">是否查找子目录</param>
        /// <returns></returns>
        public static List<DirectoryInfo> GetFolders (string path, out string error, bool searchSubFolders = false) {
            List<FileInfo> files = null;
            List<DirectoryInfo> folders = null;

            error = GetFilesAndFolders (path, ref files, ref folders,
                searchFiles : false, searchSubFolderFiles : false,
                searchFolders : true, searchSubFolders : searchSubFolders);

            return folders;
        }

        /// <summary>
        /// 遍历路径查找文件和(或)目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="foundFiles">已找到的文件列表</param>
        /// <param name="foundFolders">已找到目录列表</param>
        /// <param name="searchFiles">是否查找文件</param>
        /// <param name="searchSubFolderFiles">是否查找子目录的文件</param>
        /// <param name="searchFolders">是否查找目录</param>
        /// <param name="searchSubFolders">是否查找子目录</param>
        /// <param name="checkPath">检查文件或目录的方法</param>
        /// <returns>错误信息</returns>
        public static string GetFilesAndFolders (string path, ref List<FileInfo> foundFiles, ref List<DirectoryInfo> foundFolders,
            bool searchFiles = true, bool searchSubFolderFiles = true, bool searchFolders = true, bool searchSubFolders = true,
            Func<FileSystemInfo /*info*/ , bool /*ok*/> checkPath = null) {
            #region 参数检查
            if (!path.IsValid ()) {
                return "路径不能为空";
            }

            if (!Directory.Exists (path)) {
                return "指定的路径不存在";
            }

            if (!(searchFiles || searchSubFolderFiles || searchFolders || searchSubFolders)) {
                return "查找参数无效";
            }
            #endregion

            var dir = new DirectoryInfo (Path.GetFullPath (path));

            #region 查找文件
            if (searchFiles) {
                var files = dir.GetFiles ();
                if (!files.IsEmpty ()) {
                    if (null == foundFiles) {
                        foundFiles = new List<FileInfo> ();
                    }

                    foundFiles.AddRange (files.Where (fi => checkPath?.Invoke (fi) ?? true));
                    foundFiles.Sort ((x, y) => string.Compare (x.FullName, y.FullName));
                }
            }
            #endregion

            #region 查找目录
            var folders = dir.GetDirectories ();
            if (folders.IsEmpty ()) {
                return string.Empty;
            }

            if (searchFolders) {
                if (null == foundFolders) {
                    foundFolders = new List<DirectoryInfo> ();
                }

                foundFolders.AddRange (folders.Where (di => checkPath?.Invoke (di) ?? true));
                foundFolders.Sort ((x, y) => string.Compare (x.FullName, y.FullName));
            }
            #endregion

            if (!searchSubFolderFiles && !searchSubFolders) {
                return string.Empty;
            }

            string error = null;

            #region 递归查找子目录
            foreach (var folder in folders) {
                if (null == folder || !folder.FullName.IsValid ()) {
                    continue;
                }

                if (!(checkPath?.Invoke (folder) ?? true)) {
                    continue;
                }

                var err = GetFilesAndFolders (folder.FullName, ref foundFiles, ref foundFolders,
                    searchFiles : searchSubFolderFiles, searchSubFolderFiles : searchSubFolderFiles,
                    searchFolders : searchSubFolders, searchSubFolders : searchSubFolders,
                    checkPath : checkPath);
                if (err.IsValid ()) {
                    error = err;
                }
            }
            #endregion

            return error;
        }

        /// <summary>
        /// 获取文件的目录路径
        /// </summary>
        /// <param name="file"></param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public static string GetFileDir (string file, out string error) {
            error = string.Empty;

            if (!file.IsValid ()) {
                error = "文件路径不能为空";
                return string.Empty;
            }

            if (!File.Exists (file)) {
                error = "指定的文件不存在";
                return string.Empty;
            }

            return (new FileInfo (file)).Directory.FullName;
        }

        /// <summary>
        /// 文件路径去除文件后缀
        /// </summary>
        /// <param name="file"></param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public static string PathRemoveFileExtension (string file, out string error) {
            error = string.Empty;

            if (!file.IsValid ()) {
                error = "文件路径不能为空";
                return string.Empty;
            }

            var index = file.IndexOf (Path.GetExtension (file));
            if (index >= 0) {
                return file.Substring (0, index);
            }

            return file;
        }

        /// <summary>
        /// 获取文件大小（单位：字节）
        /// </summary>
        /// <param name="file"></param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public static long GetFileSize (string file, out string error) {
            error = string.Empty;

            if (!file.IsValid ()) {
                error = "文件路径不能为空";
                return 0;
            }

            if (!File.Exists (file)) {
                error = "指定的文件不存在";
                return 0;
            }

            return (new FileInfo (file)).Length;
        }

        /// <summary>
        /// 获取文件版本号
        /// </summary>
        /// <param name="file"></param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public static string GetFileVersion (string file, out string error) =>
            GetFileVersion (file, out error, out (int, int, int, int) versionsPart);

        /// <summary>
        /// 获取文件版本号
        /// </summary>
        /// <param name="file"></param>
        /// <param name="error">错误信息</param>
        /// <param name="versionsPart">major.minor.build.revision四个部分的数字</param>
        /// <returns></returns>
        public static string GetFileVersion (string file, out string error,
            out (int /*major*/ , int /*minor*/ , int /*build*/ , int /*revision*/ ) versionsPart) {
            error = string.Empty;
            versionsPart = (0, 0, 0, 0);

            if (!file.IsValid ()) {
                error = "文件路径不能为空";
                return string.Empty;
            }

            if (!File.Exists (file)) {
                error = "指定的文件不存在";
                return string.Empty;
            }

            var info = FileVersionInfo.GetVersionInfo (file);
            if (null == info) {
                error = "获取文件版本失败";
                return string.Empty;
            }

            versionsPart = (info.FileMajorPart, info.FileMinorPart, info.FileBuildPart, info.FilePrivatePart);

            return info.FileVersion;
        }

        /// <summary>
        /// 确认目录存在
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void EnsureDirectory (string path, bool isFile) {
            if (!path.IsValid ()) {
                return;
            }

            var dir = isFile ? Path.GetDirectoryName (path) : path;
            if (!dir.IsValid ()) {
                return;
            }

            if (!Directory.Exists (dir)) {
                Directory.CreateDirectory (dir);
            }
        }

        /// <summary>
        /// 清空目录(即删除目录下的所有文件和子目录)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="deleteAllFiles">是否删除所有文件(包含子目录下的文件)</param>
        /// <param name="deleteAllFolders">是否删除所有目录(包含子目录)</param>
        /// <returns>错误信息</returns>
        public static string ClearDirectory (string path, bool deleteAllFiles = true, bool deleteAllFolders = true) {
            try {
                if (!path.IsValid ()) {
                    return string.Empty;
                }

                List<FileInfo> files = null;
                List<DirectoryInfo> folders = null;

                var error = GetFilesAndFolders (path, ref files, ref folders, searchFiles : true,
                    searchSubFolderFiles : true, searchFolders : true, searchSubFolders : true);
                if (error.IsValid ()) {
                    return error;
                }

                if (deleteAllFiles) {
                    files?.ForEach (fi => fi.Delete ());
                }

                if (deleteAllFolders) {
                    folders?.Sort ();
                    folders?.ForEach (di => di.Delete ());
                }

                return string.Empty;
            } catch (Exception ex) {
                return ex.Message;
            }
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string ReadFile (string filePath) {
            if (!filePath.IsValid () || !File.Exists (filePath)) {
                return string.Empty;
            }

            using (var sr = new StreamReader (filePath)) {
                return sr.ReadToEnd ();
            }
        }

        /// <summary>
        /// 写入文件内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">要写入的内容</param>
        /// <param name="append">是否追加或覆盖</param>
        /// <returns></returns>
        public static string WriteFile (string filePath, string content, bool append = false) {
            if (!filePath.IsValid ()) {
                return "文件路径不能为空";
            }

            if (!content.IsValid ()) {
                return "要写入的内容不能为空";
            }

            try {
                using (var sw = new StreamWriter (filePath, append)) {
                    sw.Write (content);
                }

                return string.Empty;
            } catch (Exception ex) {
                return ex.Message;
            }
        }

        /// <summary>
        /// 复制或移动文件
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="dstFile">目标文件</param>
        /// <param name="overwrite">目标文件存在时覆盖</param>
        /// <param name="moveFile">是否移动文件(即删除原文件)</param>
        /// <returns></returns>
        public static string CopyOrMoveFile (string srcFile, string dstFile, bool overwrite = true, bool moveFile = false) {
            try {
                if (!srcFile.IsValid ()) {
                    return "原文件不能为空";
                }

                if (!dstFile.IsValid ()) {
                    return "目标文件不能为空";
                }

                if (!File.Exists (srcFile)) {
                    return "原文件不存在";
                }

                EnsureDirectory (dstFile, isFile : true);
                File.Copy (srcFile, dstFile, overwrite);

                if (moveFile) {
                    File.Delete (srcFile);
                }

                return string.Empty;
            } catch (Exception ex) {
                return ex.Message;
            }
        }

        /// <summary>
        ///  计算指定文件的MD5值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回值的字符串形式</returns>
        public static string GetFileMD5 (string filePath) {
            if (!filePath.IsValid () || !File.Exists (filePath)) {
                return string.Empty;
            }

            using (var fs = new FileStream (filePath, FileMode.Open, FileAccess.Read)) {
                return HashHelper.ComputeMD5 (fs);
            }
        }

        /// <summary>
        ///  计算指定文件的CRC32值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回值的字符串形式</returns>
        public static string GetFileCRC32 (string filePath) {
            if (!filePath.IsValid () || !File.Exists (filePath)) {
                return string.Empty;
            }

            using (var fs = new FileStream (filePath, FileMode.Open, FileAccess.Read)) {
                return HashHelper.ComputeCRC32 (fs);
            }
        }

        /// <summary>
        ///  计算指定文件的SHA1值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回值的字符串形式</returns>
        public static string GetFileSHA1 (string filePath) {
            if (!filePath.IsValid () || !File.Exists (filePath)) {
                return string.Empty;
            }

            using (var fs = new FileStream (filePath, FileMode.Open, FileAccess.Read)) {
                return HashHelper.ComputeSHA1 (fs);
            }
        }

        /// <summary>
        /// 获取文件的临时路径
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string GetTempFile (string filePath) {
            if (!filePath.IsValid ()) {
                return string.Empty;
            }

            var path = Path.GetDirectoryName (filePath);
            var file = Path.GetFileNameWithoutExtension (filePath);
            var ext = Path.GetExtension (filePath);
            var rnd = StringExtension.GetRandString (4);

            return Path.Combine (path, $"{file}_{rnd}{ext}");
        }
    }
}