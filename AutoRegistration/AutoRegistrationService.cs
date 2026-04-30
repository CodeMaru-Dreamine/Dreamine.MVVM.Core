using System;
using System.Linq;
using System.Reflection;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.AutoRegistration
{
    /// <summary>
    /// Automatically registers Dreamine-supported types.
    /// </summary>
    public sealed class AutoRegistrationService : IAutoRegistrationService
    {
        private readonly IAssemblyTypeScanner _typeScanner;
        private readonly IAutoRegistrationFilter _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoRegistrationService"/> class.
        /// </summary>
        /// <param name="typeScanner">The assembly type scanner.</param>
        /// <param name="filter">The auto-registration filter.</param>
        public AutoRegistrationService(
            IAssemblyTypeScanner typeScanner,
            IAutoRegistrationFilter filter)
        {
            _typeScanner = typeScanner ?? throw new ArgumentNullException(nameof(typeScanner));
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        /// <summary>
        /// Registers supported types from the specified root assembly.
        /// </summary>
        /// <param name="rootAssembly">The root assembly to scan first.</param>
        /// <param name="registry">The service registry to populate.</param>
        public void RegisterAll(Assembly rootAssembly, IServiceRegistry registry)
        {
            if (rootAssembly is null)
            {
                throw new ArgumentNullException(nameof(rootAssembly));
            }

            if (registry is null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            foreach (Assembly assembly in _typeScanner.GetCandidateAssemblies(rootAssembly))
            {
                RegisterAssemblyTypes(assembly, registry);
            }
        }

        private void RegisterAssemblyTypes(Assembly assembly, IServiceRegistry registry)
        {
            foreach (Type type in _typeScanner.GetLoadableTypes(assembly))
            {
                RegisterType(type, registry);
            }
        }

        private void RegisterType(Type type, IServiceRegistry registry)
        {
            if (!_filter.IsTarget(type))
            {
                return;
            }

            if (registry.IsRegistered(type))
            {
                return;
            }

            RegisterSingletonByReflection(type, registry);
        }

        private static void RegisterSingletonByReflection(Type implementationType, IServiceRegistry registry)
        {
            MethodInfo registerMethod = typeof(IServiceRegistry)
                .GetMethods()
                .Single(method =>
                    method.Name == nameof(IServiceRegistry.RegisterSingleton) &&
                    method.IsGenericMethodDefinition &&
                    method.GetGenericArguments().Length == 1 &&
                    method.GetParameters().Length == 0);

            MethodInfo closedMethod = registerMethod.MakeGenericMethod(implementationType);

            closedMethod.Invoke(registry, null);
        }
    }
}