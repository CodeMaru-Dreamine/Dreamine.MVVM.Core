using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.AutoRegistration
{
    /// <summary>
    /// Scans assemblies and returns loadable types safely.
    /// </summary>
    public sealed class AssemblyTypeScanner : IAssemblyTypeScanner
    {
        /// <summary>
        /// Gets candidate assemblies for auto registration.
        /// </summary>
        /// <param name="rootAssembly">The root assembly to prioritize.</param>
        /// <returns>The candidate assemblies.</returns>
        public IEnumerable<Assembly> GetCandidateAssemblies(Assembly rootAssembly)
        {
            if (rootAssembly is null)
            {
                throw new ArgumentNullException(nameof(rootAssembly));
            }

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.FullName))
                .Prepend(rootAssembly)
                .Distinct();
        }

        /// <summary>
        /// Gets loadable types from the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        /// <returns>The loadable types.</returns>
        public IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(type => type is not null)!;
            }
        }
    }
}