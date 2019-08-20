using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Utils {
    /// <summary>
    /// IoC窗口管理类
    /// </summary>
    public class IocManager : IIocManager {
        private IContainer _container;

        public static IocManager Instance { get { return SingletonInstance; } }
        private static readonly IocManager SingletonInstance = new IocManager ();

        /// <summary>
        /// Ioc容器初始化
        /// </summary>
        /// <param name="services"></param>
        /// <param name="allTypes">所有程序集中的类型</param>
        /// <param name="registerOthers">注册其他类型的方法</param>
        /// <returns></returns>
        public IServiceProvider Initialize (IServiceCollection services, List<Type> allTypes,
            Action<ContainerBuilder, IEnumerable<Type>> registerOthers) {
            var builder = new ContainerBuilder ();
            builder.RegisterInstance (Instance).As<IIocManager> ().SingleInstance ();

            //注册ITransientInstance实现类
            var transientType = typeof (ITransientInstance);
            var transientTypes = allTypes.Where (t => transientType.IsAssignableFrom (t) && t != transientType).ToArray ();
            builder.RegisterTypes (transientTypes)
                .AsImplementedInterfaces ()
                .InstancePerLifetimeScope ()
                .PropertiesAutowired ()
                .EnableInterfaceInterceptors ();

            foreach (Type type in transientTypes) {
                if (type.IsClass && !type.IsAbstract && !type.BaseType.IsInterface && type.BaseType != typeof (object)) {
                    builder.RegisterType (type).As (type.BaseType)
                        .InstancePerLifetimeScope ()
                        .PropertiesAutowired ();
                }
            }

            //注册ISingletonInstance实现类
            var singletonType = typeof (ISingletonInstance);
            var singletonTypes = allTypes.Where (t => singletonType.IsAssignableFrom (t) && t != singletonType).ToArray ();
            builder.RegisterTypes (singletonTypes)
                .AsImplementedInterfaces ()
                .SingleInstance ()
                .PropertiesAutowired ();

            foreach (Type type in singletonTypes) {
                if (type.IsClass && !type.IsAbstract && !type.BaseType.IsInterface && type.BaseType != typeof (object)) {
                    builder.RegisterType (type).As (type.BaseType)
                        .SingleInstance ()
                        .PropertiesAutowired ();
                }
            }

            registerOthers?.Invoke (builder, allTypes);

            builder.Populate (services);
            _container = builder.Build ();

            return new AutofacServiceProvider (_container);
        }

        /// <summary>
        /// Gets a container
        /// </summary>
        public virtual IContainer Container {
            get {
                return _container;
            }
        }

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">key</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved service</returns>
        public virtual T Resolve<T> (string key = "", ILifetimeScope scope = null) where T : class {
            if (scope == null) {
                //no scope specified
                scope = Scope ();
            }
            if (string.IsNullOrEmpty (key)) {
                return scope.Resolve<T> ();
            }
            return scope.ResolveKeyed<T> (key);
        }

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">key</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved service</returns>
        public virtual T Resolve<T> (params Parameter[] parameters) where T : class {
            var scope = Scope ();
            return scope.Resolve<T> (parameters);
        }

        /// <summary>
        /// Resolve
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved service</returns>
        public virtual object Resolve (Type type, ILifetimeScope scope = null) {
            if (scope == null) {
                //no scope specified
                scope = Scope ();
            }
            return scope.Resolve (type);
        }

        /// <summary>
        /// Resolve all
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">key</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved services</returns>
        public virtual T[] ResolveAll<T> (string key = "", ILifetimeScope scope = null) {
            if (scope == null) {
                //no scope specified
                scope = Scope ();
            }
            if (string.IsNullOrEmpty (key)) {
                return scope.Resolve<IEnumerable<T>> ().ToArray ();
            }
            return scope.ResolveKeyed<IEnumerable<T>> (key).ToArray ();
        }

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved service</returns>
        public virtual T ResolveUnregistered<T> (ILifetimeScope scope = null) where T : class {
            return ResolveUnregistered (typeof (T), scope) as T;
        }

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved service</returns>
        public virtual object ResolveUnregistered (Type type, ILifetimeScope scope = null) {
            if (scope == null) {
                //no scope specified
                scope = Scope ();
            }
            var constructors = type.GetConstructors ();
            foreach (var constructor in constructors) {
                try {
                    var parameters = constructor.GetParameters ();
                    var parameterInstances = new List<object> ();
                    foreach (var parameter in parameters) {
                        var service = Resolve (parameter.ParameterType, scope);
                        if (service == null) throw new Exception ("Unknown dependency");
                        parameterInstances.Add (service);
                    }
                    return Activator.CreateInstance (type, parameterInstances.ToArray ());
                } catch (Exception) {

                }
            }
            throw new Exception ("No constructor  was found that had all the dependencies satisfied.");
        }

        /// <summary>
        /// Try to resolve srevice
        /// </summary>
        /// <param name="serviceType">Type</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <param name="instance">Resolved service</param>
        /// <returns>Value indicating whether service has been successfully resolved</returns>
        public virtual bool TryResolve (Type serviceType, ILifetimeScope scope, out object instance) {
            if (scope == null) {
                //no scope specified
                scope = Scope ();
            }
            return scope.TryResolve (serviceType, out instance);
        }

        /// <summary>
        /// Check whether some service is registered (can be resolved)
        /// </summary>
        /// <param name="serviceType">Type</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Result</returns>
        public virtual bool IsRegistered (Type serviceType, ILifetimeScope scope = null) {
            if (scope == null) {
                //no scope specified
                scope = Scope ();
            }
            return scope.IsRegistered (serviceType);
        }

        /// <summary>
        /// Resolve optional
        /// </summary>
        /// <param name="serviceType">Type</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved service</returns>
        public virtual object ResolveOptional (Type serviceType, ILifetimeScope scope = null) {
            if (scope == null) {
                //no scope specified
                scope = Scope ();
            }
            return scope.ResolveOptional (serviceType);
        }

        /// <summary>
        /// Get current scope
        /// </summary>
        /// <returns>Scope</returns>
        public virtual ILifetimeScope Scope () {
            try {
                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope ();
            } catch (Exception) {
                //we can get an exception here if RequestLifetimeScope is already disposed
                //for example, requested in or after "Application_EndRequest" handler
                //but note that usually it should never happen

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope (MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
        }
    }
}