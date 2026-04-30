using System;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.AutoRegistration
{
    /// <summary>
    /// Determines whether a type matches Dreamine auto-registration naming conventions.
    /// </summary>
    public sealed class NamingConventionAutoRegistrationFilter : IAutoRegistrationFilter
    {
        /// <summary>
        /// Determines whether the specified type is eligible for auto registration.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>True if the type is eligible; otherwise false.</returns>
        public bool IsTarget(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!IsConcreteClass(type))
            {
                return false;
            }

            string name = type.Name;
            string fullName = type.FullName ?? string.Empty;

            return name.EndsWith("Model", StringComparison.Ordinal) ||
                   name.EndsWith("Event", StringComparison.Ordinal) ||
                   name.EndsWith("Manager", StringComparison.Ordinal) ||
                   name.EndsWith("ViewModel", StringComparison.Ordinal) ||
                   fullName.Contains(".xaml.ViewModel", StringComparison.Ordinal) ||
                   fullName.Contains(".xaml.Model", StringComparison.Ordinal) ||
                   fullName.Contains(".xaml.Event", StringComparison.Ordinal);
        }

        private static bool IsConcreteClass(Type type)
        {
            return type.IsClass &&
                   !type.IsAbstract &&
                   !type.IsGenericTypeDefinition;
        }
    }
}