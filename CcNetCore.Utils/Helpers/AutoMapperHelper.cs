#pragma warning disable CS0168

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using AutoMapper;

namespace CcNetCore.Utils.Helpers {
    /// <summary>
    /// AutoMapper扩展帮助类
    /// </summary>
    public static class AutoMapperHelper {
        /// <summary>
        /// 创建临时映射
        /// </summary>
        /// <returns></returns>
        public static IMapper CreateMapper (Action<IMapperConfigurationExpression> configure) =>
            new MapperConfiguration (configure).CreateMapper ();

        /// <summary>
        /// 创建临时映射
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        public static IMapper CreateMapper<TSource, TDestination> () =>
            CreateMapper (cfg => cfg.CreateMap<TSource, TDestination> ());

        /// <summary>
        /// 创建临时映射
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public static IMapper CreateMapper (Type sourceType, Type destinationType) =>
            CreateMapper (cfg => cfg.CreateMap (sourceType, destinationType));

        /// <summary>
        ///  类型映射
        /// </summary>
        public static T MapTo<T> (this object obj) {
            if (obj == null) {
                return default (T);
            }

            return CreateMapper (obj.GetType (), typeof (T)).Map<T> (obj);
        }

        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TDestination> (this IEnumerable source) {
            var mapper = CreateMapper (cfg => {
                foreach (var first in source) {
                    var type = first.GetType ();
                    cfg.CreateMap (type, typeof (TDestination));
                    break;
                }
            });

            return mapper.Map<List<TDestination>> (source);
        }

        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TSource, TDestination> (this IEnumerable<TSource> source) =>
            CreateMapper<TSource, TDestination> ().Map<List<TDestination>> (source);

        /// <summary>
        /// 类型映射
        /// </summary>
        public static TDestination MapTo<TSource, TDestination> (this TSource source, TDestination destination)
        where TSource : class
        where TDestination : class {
            if (source == null) {
                return destination;
            }

            return CreateMapper<TSource, TDestination> ().Map (source, destination);
        }

        /// <summary>
        /// DataReader映射
        /// </summary>
        public static IEnumerable<T> DataReaderMapTo<T> (this IDataReader reader) =>
            CreateMapper<IDataReader, IEnumerable<T>> ().Map<IDataReader, IEnumerable<T>> (reader);
    }
}