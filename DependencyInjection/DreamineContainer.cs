using System;
using System.Collections.Generic;
using System.Threading;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// \if KO
    /// <para>Dreamine Container 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Default dependency container implementation for Dreamine.</para>
    /// \endif
    /// </summary>
    public sealed class DreamineContainer : IServiceContainer, IDisposable
    {
        /// <summary>
        /// \if KO
        /// <para>descriptors 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the descriptors value.</para>
        /// \endif
        /// </summary>
        private readonly Dictionary<Type, ServiceDescriptor> _descriptors = new();
        /// <summary>
        /// \if KO
        /// <para>singleton Instances 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the singleton instances value.</para>
        /// \endif
        /// </summary>
        private readonly Dictionary<Type, object> _singletonInstances = new();
        /// <summary>
        /// \if KO
        /// <para>current Resolution Context 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the current resolution context value.</para>
        /// \endif
        /// </summary>
        private readonly AsyncLocal<ResolutionContext?> _currentResolutionContext = new();
        /// <summary>
        /// \if KO
        /// <para>object Activator 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the object activator value.</para>
        /// \endif
        /// </summary>
        private readonly IObjectActivator _objectActivator;
        /// <summary>
        /// \if KO
        /// <para>sync Root 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the sync root value.</para>
        /// \endif
        /// </summary>
        private readonly object _syncRoot = new();
        /// <summary>
        /// \if KO
        /// <para>disposed 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the disposed value.</para>
        /// \endif
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// \if KO
        /// <para>지정한 설정으로 <see cref="DreamineContainer"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes a new instance of the <see cref="DreamineContainer"/> class.</para>
        /// \endif
        /// </summary>
        public DreamineContainer()
            : this(new ConstructorActivator(new ConstructorSelector()))
        {
        }

        /// <summary>
        /// \if KO
        /// <para>지정한 설정으로 <see cref="DreamineContainer"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes a new instance of the <see cref="DreamineContainer"/> class.</para>
        /// \endif
        /// </summary>
        /// <param name="objectActivator">
        /// \if KO
        /// <para>object Activator에 사용할 <see cref="IObjectActivator"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The object activator.</para>
        /// \endif
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para>필수 입력 인자 중 하나가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when a required input argument is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
        public DreamineContainer(IObjectActivator objectActivator)
        {
            _objectActivator = objectActivator
                ?? throw new ArgumentNullException(nameof(objectActivator));
        }

        /// <summary>
        /// \if KO
        /// <para>Register 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers a concrete implementation type as itself.</para>
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
        public void Register<TImplementation>()
            where TImplementation : class
        {
            Register<TImplementation, TImplementation>();
        }

        /// <summary>
        /// \if KO
        /// <para>Register 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers a service abstraction with a concrete implementation.</para>
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
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            Type serviceType = typeof(TService);
            Type implementationType = typeof(TImplementation);

            lock (_syncRoot)
            {
                _descriptors[serviceType] = new ServiceDescriptor(
                    serviceType,
                    implementationType,
                    factory: null,
                    instance: null,
                    lifetime: ServiceLifetime.Transient);
                _singletonInstances.Remove(serviceType);
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Register 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers a factory for the specified service type.</para>
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
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para>필수 입력 인자 중 하나가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when a required input argument is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
        public void Register<TService>(Func<TService> factory)
            where TService : class
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Type serviceType = typeof(TService);

            lock (_syncRoot)
            {
                _descriptors[serviceType] = new ServiceDescriptor(
                    serviceType,
                    implementationType: null,
                    factory: () => factory(),
                    instance: null,
                    lifetime: ServiceLifetime.Transient);
                _singletonInstances.Remove(serviceType);
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
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para>필수 입력 인자 중 하나가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when a required input argument is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
        public void RegisterSingleton<TService>(TService instance)
            where TService : class
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            Type serviceType = typeof(TService);

            lock (_syncRoot)
            {
                _descriptors[serviceType] = new ServiceDescriptor(
                    serviceType,
                    instance.GetType(),
                    factory: null,
                    instance,
                    lifetime: ServiceLifetime.Singleton);

                _singletonInstances[serviceType] = instance;
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
        public void RegisterSingleton<TImplementation>()
            where TImplementation : class
        {
            RegisterSingleton<TImplementation, TImplementation>();
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
        public void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            Type serviceType = typeof(TService);
            Type implementationType = typeof(TImplementation);

            lock (_syncRoot)
            {
                _descriptors[serviceType] = new ServiceDescriptor(
                    serviceType,
                    implementationType,
                    factory: null,
                    instance: null,
                    lifetime: ServiceLifetime.Singleton);
                _singletonInstances.Remove(serviceType);
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
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para>필수 입력 인자 중 하나가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when a required input argument is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
        public bool IsRegistered(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            lock (_syncRoot)
            {
                return _descriptors.ContainsKey(serviceType);
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
        public TService Resolve<TService>()
            where TService : class
        {
            return (TService)Resolve(typeof(TService));
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
        public bool TryResolve<TService>(out TService? result)
            where TService : class
        {
            try
            {
                result = Resolve<TService>();
                return true;
            }
            catch (InvalidOperationException)
            {
                result = null;
                return false;
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
        /// <para>Resolve 작업에서 생성한 <see cref="object"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The resolved service instance.</para>
        /// \endif
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para>필수 입력 인자 중 하나가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when a required input argument is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
        public object Resolve(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            // AsyncLocal context setup is per-async-flow and must live outside the lock
            // so that nested Resolve calls on the same flow see the same context
            // without requiring lock re-entry ordering to be preserved.
            ResolutionContext? previousContext = _currentResolutionContext.Value;
            bool ownsContext = previousContext is null;
            if (ownsContext)
            {
                _currentResolutionContext.Value = new ResolutionContext();
            }

            try
            {
                lock (_syncRoot)
                {
                    return ResolveCore(serviceType);
                }
            }
            finally
            {
                if (ownsContext)
                {
                    _currentResolutionContext.Value = null;
                }
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Resolve Core 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the resolve core operation.</para>
        /// \endif
        /// </summary>
        /// <param name="serviceType">
        /// \if KO
        /// <para>service Type에 사용할 <see cref="Type"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="Type"/> value used for service type.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Resolve Core 작업에서 생성한 <see cref="object"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="object"/> result produced by the resolve core operation.</para>
        /// \endif
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// \if KO
        /// <para>현재 객체 상태에서 Resolve Core 작업을 수행할 수 없는 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when the resolve core operation is not valid for the current object state.</para>
        /// \endif
        /// </exception>
        private object ResolveCore(Type serviceType)
        {
            if (_singletonInstances.TryGetValue(serviceType, out object? singleton))
            {
                return singleton;
            }

            if (!_descriptors.TryGetValue(serviceType, out ServiceDescriptor? descriptor))
            {
                if (CanCreateUnregisteredConcreteType(serviceType))
                {
                    return CreateConcrete(serviceType);
                }

                throw new InvalidOperationException(
                    $"Service [{serviceType.FullName}] is not registered.");
            }

            if (descriptor.Factory is not null)
            {
                return descriptor.Factory();
            }

            if (descriptor.Instance is not null)
            {
                return descriptor.Instance;
            }

            if (descriptor.ImplementationType is null)
            {
                throw new InvalidOperationException(
                    $"Service [{serviceType.FullName}] has no implementation type.");
            }

            object instance = CreateConcrete(descriptor.ImplementationType);

            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                _singletonInstances[serviceType] = instance;
            }

            return instance;
        }

        /// <summary>
        /// \if KO
        /// <para>Concrete 값을 생성합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Creates the concrete value.</para>
        /// \endif
        /// </summary>
        /// <param name="implementationType">
        /// \if KO
        /// <para>implementation Type에 사용할 <see cref="Type"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="Type"/> value used for implementation type.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Create Concrete 작업에서 생성한 <see cref="object"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="object"/> result produced by the create concrete operation.</para>
        /// \endif
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// \if KO
        /// <para>현재 객체 상태에서 Create Concrete 작업을 수행할 수 없는 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when the create concrete operation is not valid for the current object state.</para>
        /// \endif
        /// </exception>
        private object CreateConcrete(Type implementationType)
        {
            ResolutionContext context = _currentResolutionContext.Value ?? new ResolutionContext();
            if (!context.TryEnter(implementationType))
            {
                throw new InvalidOperationException(
                    $"Circular dependency detected while resolving [{implementationType.FullName}].");
            }

            try
            {
                return _objectActivator.CreateInstance(implementationType, this);
            }
            finally
            {
                context.Exit(implementationType);
            }
        }

        /// <summary>
        /// \if KO
        /// <para>이 인스턴스가 소유한 리소스를 해제합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Disposes all singleton instances that implement <see cref="IDisposable"/>, then clears all registrations.</para>
        /// \endif
        /// </summary>
        public void Dispose()
        {
            List<object> toDispose;

            lock (_syncRoot)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                toDispose = new List<object>(_singletonInstances.Values);
                _singletonInstances.Clear();
                _descriptors.Clear();
            }

            foreach (object instance in toDispose)
            {
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Can Create Unregistered Concrete Type 조건을 확인합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Determines whether can create unregistered concrete type.</para>
        /// \endif
        /// </summary>
        /// <param name="type">
        /// \if KO
        /// <para>type에 사용할 <see cref="Type"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="Type"/> value used for type.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Can Create Unregistered Concrete Type 조건이 충족되면 <see langword="true"/>이고, 그렇지 않으면 <see langword="false"/>입니다.</para>
        /// \endif
        /// \if EN
        /// <para><see langword="true"/> when the can create unregistered concrete type condition is satisfied; otherwise, <see langword="false"/>.</para>
        /// \endif
        /// </returns>
        private static bool CanCreateUnregisteredConcreteType(Type type)
        {
            return type.IsClass &&
                   !type.IsAbstract &&
                   !type.IsGenericTypeDefinition;
        }
    }
}
