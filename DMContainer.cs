using System;
using System.Reflection;
using Dreamine.MVVM.Core.AutoRegistration;
using Dreamine.MVVM.Core.DependencyInjection;

namespace Dreamine.MVVM.Core
{
    /// <summary>
    /// Provides a static facade for the default Dreamine dependency container.
    /// </summary>
    public static partial class DMContainer
    {
        private static readonly DreamineContainer Container = new();

        private static readonly AutoRegistrationService AutoRegistrationService = new(
            new AssemblyTypeScanner(),
            new NamingConventionAutoRegistrationFilter());

        /// <summary>
        /// Registers a concrete implementation type as itself with transient lifetime.
        /// </summary>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public static void Register<TImplementation>()
            where TImplementation : class
        {
            Container.Register<TImplementation>();
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
            Container.Register<TService, TImplementation>();
        }

        /// <summary>
        /// Registers a factory for the specified service type using transient lifetime.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">The factory used to create the service instance.</param>
        public static void Register<TService>(Func<TService> factory)
            where TService : class
        {
            Container.Register(factory);
        }

        /// <summary>
        /// Registers a singleton instance for the specified service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="instance">The singleton instance.</param>
        public static void RegisterSingleton<TService>(TService instance)
            where TService : class
        {
            Container.RegisterSingleton(instance);
        }

        /// <summary>
        /// Registers a concrete implementation type as itself with singleton lifetime.
        /// </summary>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public static void RegisterSingleton<TImplementation>()
            where TImplementation : class
        {
            Container.RegisterSingleton<TImplementation>();
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
            Container.RegisterSingleton<TService, TImplementation>();
        }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>The resolved service instance.</returns>
        public static TService Resolve<TService>()
            where TService : class
        {
            return Container.Resolve<TService>();
        }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <returns>The resolved service instance.</returns>
        public static object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        /// <summary>
        /// Automatically registers supported types from the specified root assembly using singleton lifetime.
        /// </summary>
        /// <param name="rootAssembly">The root assembly to scan first.</param>
        public static void AutoRegisterAll(Assembly rootAssembly)
        {
            AutoRegistrationService.RegisterAll(rootAssembly, Container);
        }

        /// <summary>
        /// Determines whether the specified service type is registered.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>True if the service type is registered; otherwise false.</returns>
        public static bool IsRegistered(Type serviceType)
        {
            return Container.IsRegistered(serviceType);
        }
    }
}