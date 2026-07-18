using System;
using System.Reflection;
using Dreamine.MVVM.Core.DependencyInjection;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core
{
    /// <summary>
    /// \if KO
    /// <para>DM Container 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Provides a static facade for the default Dreamine dependency container.</para>
    /// \endif
    /// </summary>
    public static partial class DMContainer
    {
        /// <summary>
        /// \if KO
        /// <para>Sync Root 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the sync root value.</para>
        /// \endif
        /// </summary>
        private static readonly object SyncRoot = new();
        /// <summary>
        /// \if KO
        /// <para>Container 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the container value.</para>
        /// \endif
        /// </summary>
        private static IServiceContainer Container = CreateDefaultContainer();

        // Self-registers IServiceResolver so that ViewManager(IServiceResolver) and other
        // constructor-injected types can be resolved by DreamineContainer without an
        // explicit registration call from application startup code.
        /// <summary>
        /// \if KO
        /// <para>Default Container 값을 생성합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Creates the default container value.</para>
        /// \endif
        /// </summary>
        /// <returns>
        /// \if KO
        /// <para>Create Default Container 작업에서 생성한 <see cref="DreamineContainer"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="DreamineContainer"/> result produced by the create default container operation.</para>
        /// \endif
        /// </returns>
        private static DreamineContainer CreateDefaultContainer()
        {
            var c = new DreamineContainer();
            c.RegisterSingleton<IServiceResolver>(c);
            return c;
        }

        /// <summary>
        /// \if KO
        /// <para>Registry 값을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Returns the underlying <see cref="IServiceRegistry"/> so that external utilities (e.g. <see cref="DreamineAutoRegistrar"/>) can register types without going through the static facade methods.</para>
        /// \endif
        /// </summary>
        /// <returns>
        /// \if KO
        /// <para>Get Registry 작업에서 생성한 <see cref="IServiceRegistry"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="IServiceRegistry"/> result produced by the get registry operation.</para>
        /// \endif
        /// </returns>
        internal static IServiceRegistry GetRegistry()
        {
            lock (SyncRoot) { return Container; }
        }

        /// <summary>
        /// \if KO
        /// <para>Resolver 값을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Returns the underlying <see cref="IServiceResolver"/> for constructor injection scenarios where callers want to avoid a direct static dependency on <see cref="DMContainer"/>.</para>
        /// \endif
        /// </summary>
        /// <returns>
        /// \if KO
        /// <para>Get Resolver 작업에서 생성한 <see cref="IServiceResolver"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="IServiceResolver"/> result produced by the get resolver operation.</para>
        /// \endif
        /// </returns>
        public static IServiceResolver GetResolver()
        {
            lock (SyncRoot) { return Container; }
        }


        /// <summary>
        /// \if KO
        /// <para>Container 값을 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Replaces the default container used by the static facade.</para>
        /// \endif
        /// </summary>
        /// <param name="container">
        /// \if KO
        /// <para>container에 사용할 <see cref="IServiceContainer"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The container to use for subsequent registrations and resolutions.</para>
        /// \endif
        /// </param>
        public static void SetContainer(IServiceContainer container)
        {
            ArgumentNullException.ThrowIfNull(container);

            lock (SyncRoot)
            {
                Container = container;
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Reset 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Resets the static facade to a new empty <see cref="DreamineContainer"/> instance.</para>
        /// \endif
        /// </summary>
        public static void Reset()
        {
            IServiceContainer previous;

            lock (SyncRoot)
            {
                previous = Container;
                Container = CreateDefaultContainer();
            }

            (previous as IDisposable)?.Dispose();
        }

        /// <summary>
        /// \if KO
        /// <para>Register 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers a concrete implementation type as itself with transient lifetime.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TImplementation">
        /// \if KO
        /// <para>TImplementation 형식 매개변수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The concrete implementation type.</para>
        /// \endif
        /// </typeparam>
        public static void Register<TImplementation>()
            where TImplementation : class
        {
            lock (SyncRoot)
            {
                Container.Register<TImplementation>();
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Register 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers a service abstraction with a concrete implementation using transient lifetime.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TService">
        /// \if KO
        /// <para>TService 형식 매개변수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service abstraction type.</para>
        /// \endif
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// \if KO
        /// <para>TImplementation 형식 매개변수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The concrete implementation type.</para>
        /// \endif
        /// </typeparam>
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
        /// \if KO
        /// <para>Register 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers a factory for the specified service type using transient lifetime.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TService">
        /// \if KO
        /// <para>TService 형식 매개변수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service type.</para>
        /// \endif
        /// </typeparam>
        /// <param name="factory">
        /// \if KO
        /// <para>factory에 사용할 <c>Func&lt;TService&gt;</c> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The factory used to create the service instance.</para>
        /// \endif
        /// </param>
        public static void Register<TService>(Func<TService> factory)
            where TService : class
        {
            lock (SyncRoot)
            {
                Container.Register(factory);
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Register Singleton 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers a singleton instance for the specified service type.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TService">
        /// \if KO
        /// <para>TService 형식 매개변수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service type.</para>
        /// \endif
        /// </typeparam>
        /// <param name="instance">
        /// \if KO
        /// <para>instance에 사용할 <typeparamref name="TService"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The singleton instance.</para>
        /// \endif
        /// </param>
        public static void RegisterSingleton<TService>(TService instance)
            where TService : class
        {
            lock (SyncRoot)
            {
                Container.RegisterSingleton(instance);
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Register Singleton 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers a concrete implementation type as itself with singleton lifetime.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TImplementation">
        /// \if KO
        /// <para>TImplementation 형식 매개변수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The concrete implementation type.</para>
        /// \endif
        /// </typeparam>
        public static void RegisterSingleton<TImplementation>()
            where TImplementation : class
        {
            lock (SyncRoot)
            {
                Container.RegisterSingleton<TImplementation>();
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Register Singleton 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers a service abstraction with a concrete implementation using singleton lifetime.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TService">
        /// \if KO
        /// <para>TService 형식 매개변수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service abstraction type.</para>
        /// \endif
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// \if KO
        /// <para>TImplementation 형식 매개변수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The concrete implementation type.</para>
        /// \endif
        /// </typeparam>
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
        /// \if KO
        /// <para>Resolve 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Resolves an instance of the specified service type.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TService">
        /// \if KO
        /// <para>TService 형식 매개변수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service type.</para>
        /// \endif
        /// </typeparam>
        /// <returns>
        /// \if KO
        /// <para>Resolve 작업에서 생성한 <typeparamref name="TService"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The resolved service instance.</para>
        /// \endif
        /// </returns>
        public static TService Resolve<TService>()
            where TService : class
        {
            lock (SyncRoot)
            {
                return Container.Resolve<TService>();
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Resolve 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Resolves an instance of the specified service type.</para>
        /// \endif
        /// </summary>
        /// <param name="type">
        /// \if KO
        /// <para>type에 사용할 <see cref="Type"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service type.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Resolve 작업에서 생성한 <see cref="object"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The resolved service instance.</para>
        /// \endif
        /// </returns>
        public static object Resolve(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            lock (SyncRoot)
            {
                return Container.Resolve(type);
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Resolve 작업을 시도하고 성공 여부를 반환합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Attempts to resolve an instance of the specified service type without throwing.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TService">
        /// \if KO
        /// <para>TService 형식 매개변수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service type.</para>
        /// \endif
        /// </typeparam>
        /// <param name="result">
        /// \if KO
        /// <para>result에 사용할 <typeparamref name="TService"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The resolved instance, or <c>null</c> if not registered.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Try Resolve 조건이 충족되면 <see langword="true"/>이고, 그렇지 않으면 <see langword="false"/>입니다.</para>
        /// \endif
        /// \if EN
        /// <para><c>true</c> if resolved successfully; otherwise <c>false</c>.</para>
        /// \endif
        /// </returns>
        public static bool TryResolve<TService>(out TService? result)
            where TService : class
        {
            try
            {
                lock (SyncRoot)
                {
                    result = Container.Resolve<TService>();
                    return true;
                }
            }
            catch (InvalidOperationException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Auto Register All 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Automatically registers supported types from the specified root assembly using singleton lifetime.</para>
        /// \endif
        /// </summary>
        /// <param name="rootAssembly">
        /// \if KO
        /// <para>root Assembly에 사용할 <see cref="Assembly"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The root assembly to scan first.</para>
        /// \endif
        /// </param>
        /// <remarks>
        /// \if KO
        /// <para>이 멤버의 동작과 사용 시 고려 사항을 설명합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Prefer <see cref="DreamineAutoRegistrar.RegisterAll(Assembly, IServiceRegistry)"/> for new code. This overload is kept for backward compatibility.</para>
        /// \endif
        /// </remarks>
        [System.Obsolete("Use DreamineAutoRegistrar.RegisterAll(rootAssembly, DMContainer) instead.")]
        public static void AutoRegisterAll(Assembly rootAssembly)
        {
            DreamineAutoRegistrar.RegisterAll(rootAssembly, Container);
        }

        /// <summary>
        /// \if KO
        /// <para>Is Registered 조건을 확인합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Determines whether the specified service type is registered.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TService">
        /// \if KO
        /// <para>TService 형식 매개변수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service type.</para>
        /// \endif
        /// </typeparam>
        /// <returns>
        /// \if KO
        /// <para>Is Registered 조건이 충족되면 <see langword="true"/>이고, 그렇지 않으면 <see langword="false"/>입니다.</para>
        /// \endif
        /// \if EN
        /// <para>True if the service type is registered; otherwise false.</para>
        /// \endif
        /// </returns>
        public static bool IsRegistered<TService>()
        {
            lock (SyncRoot)
            {
                return Container.IsRegistered(typeof(TService));
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Is Registered 조건을 확인합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Determines whether the specified service type is registered.</para>
        /// \endif
        /// </summary>
        /// <param name="serviceType">
        /// \if KO
        /// <para>service Type에 사용할 <see cref="Type"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service type.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Is Registered 조건이 충족되면 <see langword="true"/>이고, 그렇지 않으면 <see langword="false"/>입니다.</para>
        /// \endif
        /// \if EN
        /// <para>True if the service type is registered; otherwise false.</para>
        /// \endif
        /// </returns>
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
