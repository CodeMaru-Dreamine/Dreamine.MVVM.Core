using System;
using System.Collections.Generic;
using System.Threading;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// Default dependency container implementation for Dreamine.
    /// </summary>
    public sealed class DreamineContainer : IServiceContainer
    {
        private readonly Dictionary<Type, ServiceDescriptor> _descriptors = new();
        private readonly Dictionary<Type, object> _singletonInstances = new();
        private readonly AsyncLocal<ResolutionContext?> _currentResolutionContext = new();
        private readonly IObjectActivator _objectActivator;
        private readonly object _syncRoot = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DreamineContainer"/> class.
        /// </summary>
        public DreamineContainer()
            : this(new ConstructorActivator(new ConstructorSelector()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DreamineContainer"/> class.
        /// </summary>
        /// <param name="objectActivator">The object activator.</param>
        public DreamineContainer(IObjectActivator objectActivator)
        {
            _objectActivator = objectActivator
                ?? throw new ArgumentNullException(nameof(objectActivator));
        }

        /// <summary>
        /// Registers a concrete implementation type as itself.
        /// </summary>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public void Register<TImplementation>()
            where TImplementation : class
        {
            Register<TImplementation, TImplementation>();
        }

        /// <summary>
        /// Registers a service abstraction with a concrete implementation.
        /// </summary>
        /// <typeparam name="TService">The service abstraction type.</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
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
        /// Registers a factory for the specified service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">The factory used to create the service instance.</param>
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
        /// Registers a singleton instance for the specified service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="instance">The singleton instance.</param>
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
        /// Registers a concrete implementation type as itself with singleton lifetime.
        /// </summary>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public void RegisterSingleton<TImplementation>()
            where TImplementation : class
        {
            RegisterSingleton<TImplementation, TImplementation>();
        }

        /// <summary>
        /// Registers a service abstraction with a concrete implementation using singleton lifetime.
        /// </summary>
        /// <typeparam name="TService">The service abstraction type.</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
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
        /// Determines whether the specified service type is registered.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>True if the service type is registered; otherwise false.</returns>
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
        /// Resolves an instance of the specified service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>The resolved service instance.</returns>
        public TService Resolve<TService>()
            where TService : class
        {
            return (TService)Resolve(typeof(TService));
        }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The resolved service instance.</returns>
        public object Resolve(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            lock (_syncRoot)
            {
                ResolutionContext? previousContext = _currentResolutionContext.Value;
                bool ownsContext = previousContext is null;
                if (ownsContext)
                {
                    _currentResolutionContext.Value = new ResolutionContext();
                }

                try
                {
                    return ResolveCore(serviceType);
                }
                finally
                {
                    if (ownsContext)
                    {
                        _currentResolutionContext.Value = null;
                    }
                }
            }
        }

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

        private static bool CanCreateUnregisteredConcreteType(Type type)
        {
            return type.IsClass &&
                   !type.IsAbstract &&
                   !type.IsGenericTypeDefinition;
        }
    }
}
