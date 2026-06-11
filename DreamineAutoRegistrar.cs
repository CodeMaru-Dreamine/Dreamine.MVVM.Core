using System.Reflection;
using Dreamine.MVVM.Core.AutoRegistration;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core
{
    /// <summary>
    /// Scans assemblies and registers matching types into a service registry.
    /// Separated from <see cref="DMContainer"/> to keep the DI facade focused on
    /// registration and resolution, not type scanning.
    /// </summary>
    public static class DreamineAutoRegistrar
    {
        private static readonly AutoRegistrationService Service = new(
            new AssemblyTypeScanner(),
            new NamingConventionAutoRegistrationFilter());

        /// <summary>
        /// Scans <paramref name="rootAssembly"/> and all transitively referenced
        /// Dreamine assemblies for types that match the naming convention, then
        /// registers them into <paramref name="registry"/> with singleton lifetime.
        /// </summary>
        /// <param name="rootAssembly">The assembly to scan first.</param>
        /// <param name="registry">The registry to register discovered types into.</param>
        public static void RegisterAll(Assembly rootAssembly, IServiceRegistry registry)
        {
            Service.RegisterAll(rootAssembly, registry);
        }

        /// <summary>
        /// Scans <paramref name="rootAssembly"/> and registers matching types into
        /// the global <see cref="DMContainer"/> singleton.
        /// </summary>
        /// <param name="rootAssembly">The assembly to scan first.</param>
        public static void RegisterAll(Assembly rootAssembly)
        {
            Service.RegisterAll(rootAssembly, DMContainer.GetRegistry());
        }
    }
}
