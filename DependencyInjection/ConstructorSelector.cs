using System;
using System.Linq;
using System.Reflection;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// Selects the constructor with the largest number of parameters.
    /// </summary>
    public sealed class ConstructorSelector : IConstructorSelector
    {
        /// <summary>
        /// Selects the constructor to use for the specified implementation type.
        /// </summary>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>The selected constructor.</returns>
        public ConstructorInfo SelectConstructor(Type implementationType)
        {
            if (implementationType is null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            ConstructorInfo? constructor = implementationType
                .GetConstructors()
                .OrderByDescending(item => item.GetParameters().Length)
                .FirstOrDefault();

            if (constructor is null)
            {
                throw new InvalidOperationException(
                    $"No public constructor was found for [{implementationType.FullName}].");
            }

            return constructor;
        }
    }
}