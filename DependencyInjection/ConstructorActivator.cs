using System;
using System.Linq;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// Creates object instances by resolving constructor parameters.
    /// </summary>
    public sealed class ConstructorActivator : IObjectActivator
    {
        private readonly IConstructorSelector _constructorSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorActivator"/> class.
        /// </summary>
        /// <param name="constructorSelector">The constructor selector.</param>
        public ConstructorActivator(IConstructorSelector constructorSelector)
        {
            _constructorSelector = constructorSelector
                ?? throw new ArgumentNullException(nameof(constructorSelector));
        }

        /// <summary>
        /// Creates an instance of the specified implementation type.
        /// </summary>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="resolver">The resolver used to resolve constructor dependencies.</param>
        /// <returns>The created object instance.</returns>
        public object CreateInstance(Type implementationType, IServiceResolver resolver)
        {
            if (implementationType is null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            var constructor = _constructorSelector.SelectConstructor(implementationType);

            object[] arguments = constructor
                .GetParameters()
                .Select(parameter => resolver.Resolve(parameter.ParameterType))
                .ToArray();

            return Activator.CreateInstance(implementationType, arguments)
                ?? throw new InvalidOperationException(
                    $"Failed to create instance of [{implementationType.FullName}].");
        }
    }
}