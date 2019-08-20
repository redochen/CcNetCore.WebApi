using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CcNetCore.Utils.Extensions {
    /// <summary>
    /// 程序集扩展类
    /// </summary>
    public static class AssemblyExtension {
        /// <summary>
        /// 获取所有含有实现T接口类的程序集
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="includeSelf">是否包含自身程序集</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Assembly> GetAssignableAssemblies<T> (this Assembly assembly, bool includeSelf) {
            var assemblies = new List<Assembly> ();

            if (includeSelf) {
                assemblies.Add (assembly);
            }

            //获取所有引用程序集
            assemblies.AddRange (assembly.GetReferencedAssemblies ().Select (Assembly.Load));

            assemblies = assemblies.Where (a => a.DefinedTypes.Any (
                t => typeof (T).IsAssignableFrom (t.AsType ())))?.ToList ();

            return assemblies;
        }
    }
}