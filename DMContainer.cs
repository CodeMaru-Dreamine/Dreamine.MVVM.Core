using System;
using System.Reflection;
using Dreamine.MVVM.Core.AutoRegistration;
using Dreamine.MVVM.Core.DependencyInjection;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core
{
    /// <summary>
    /// Provides a static facade for the default Dreamine dependency container.
    /// </summary>
    public static partial class DMContainer
    {
        private static readonly object SyncRoot = new();
        private static IServiceContainer Container = new DreamineContainer();

        private static readonly AutoRegistrationService AutoRegistrationService = new(
            new AssemblyTypeScanner(),
            new NamingConventionAutoRegistrationFilter());

        /// <summary>
        /// Replaces the default container used by the static facade.
        /// </summary>
        /// <param name="container">The container to use for subsequent registrations and resolutions.</param>
        public static void SetContainer(IServiceContainer container)
        {
            ArgumentNullException.ThrowIfNull(container);

            lock (SyncRoot)
            {
                Container = container;
            }
        }

        /// <summary>
        /// Resets the static facade to a new empty <see cref="DreamineContainer"/> instance.
        /// </summary>
        public static void Reset()
        {
            lock (SyncRoot)
            {
                Container = new DreamineContainer();
            }
        }

        /// <summary>
        /// Registers a concrete implementation type as itself with transient lifetime.
        /// </summary>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public static void Register<TImplementation>()
            where TImplementation : class
        {
            lock (SyncRoot)
            {
                Container.Register<TImplementation>();
            }
        }

        /// <summary>
        /// Registers a service abstraction with a concrete implementation using transient lifetime.
        /// </summary>
        /// <typeparam name="TService">The service abstraction type.</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public static void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            lock (SyncRoot)
            {
                Container.Register<TService, TImplementation>();
            }
        }

        /// <summary>
        /// Registers a factory for the specified service type using transient lifetime.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">The factory used to create the service instance.</param>
        public static void Register<TService>(Func<TService> factory)
            where TService : class
        {
            lock (SyncRoot)
            {
                Container.Register(factory);
            }
        }

        /// <summary>
        /// Registers a singleton instance for the specified service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="instance">The singleton instance.</param>
        public static void RegisterSingleton<TService>(TService instance)
            where TService : class
        {
            lock (SyncRoot)
            {
                Container.RegisterSingleton(instance);
            }
        }

        /// <summary>
        /// Registers a concrete implementation type as itself with singleton lifetime.
        /// </summary>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public static void RegisterSingleton<TImplementation>()
            where TImplementation : class
        {
            lock (SyncRoot)
            {
                Container.RegisterSingleton<TImplementation>();
            }
        }

        /// <summary>
        /// Registers a service abstraction with a concrete implementation using singleton lifetime.
        /// </summary>
        /// <typeparam name="TService">The service abstraction type.</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public static void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            lock (SyncRoot)
            {
                Container.RegisterSingleton<TService, TImplementation>();
            }
        }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>The resolved service instance.</returns>
        public static TService Resolve<TService>()
            where TService : class
        {
            lock (SyncRoot)
            {
                return Container.Resolve<TService>();
            }
        }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <returns>The resolved service instance.</returns>
        public static object Resolve(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            lock (SyncRoot)
            {
                return Container.Resolve(type);
            }
        }

        /// <summary>
        /// Automatically registers supported types from the specified root assembly using singleton lifetime.
        /// </summary>
        /// <param name="rootAssembly">The root assembly to scan first.</param>
        public static void AutoRegisterAll(Assembly rootAssembly)
        {
            lock (SyncRoot)
            {
                AutoRegistrationService.RegisterAll(rootAssembly, Container);
            }
        }

        /// <summary>
        /// Determines whether the specified service type is registered.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>True if the service type is registered; otherwise false.</returns>
        public static bool IsRegistered<TService>()
        {
            lock (SyncRoot)
            {
                return Container.IsRegistered(typeof(TService));
            }
        }

        /// <summary>
        /// Determines whether the specified service type is registered.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>True if the service type is registered; otherwise false.</returns>
        public static bool IsRegistered(Type serviceType)
        {
            ArgumentNullException.ThrowIfNull(serviceType);

            lock (SyncRoot)
            {
                return Container.IsRegistered(serviceType);
            }
        }
    }
}
