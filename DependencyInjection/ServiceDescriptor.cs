using System;

namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// Describes a registered service.
    /// </summary>
    public sealed class ServiceDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDescriptor"/> class.
        /// </summary>
        /// <param name="serviceType">The service abstraction type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="factory">The optional factory.</param>
        /// <param name="instance">The optional singleton instance.</param>
        /// <param name="lifetime">The service lifetime.</param>
        public ServiceDescriptor(
            Type serviceType,
            Type? implementationType,
            Func<object>? factory,
            object? instance,
            ServiceLifetime lifetime)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            ImplementationType = implementationType;
            Factory = factory;
            Instance = instance;
            Lifetime = lifetime;
        }

        /// <summary>
        /// Gets the service abstraction type.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Gets the concrete implementation type.
        /// </summary>
        public Type? ImplementationType { get; }

        /// <summary>
        /// Gets the factory used to create the service instance.
        /// </summary>
        public Func<object>? Factory { get; }

        /// <summary>
        /// Gets the registered singleton instance.
        /// </summary>
        public object? Instance { get; }

        /// <summary>
        /// Gets the service lifetime.
        /// </summary>
        public ServiceLifetime Lifetime { get; }
    }
}