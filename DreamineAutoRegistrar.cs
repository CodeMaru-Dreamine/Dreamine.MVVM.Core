using System.Reflection;
using Dreamine.MVVM.Core.AutoRegistration;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core
{
    /// <summary>
    /// \if KO
    /// <para>Dreamine Auto Registrar 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Scans assemblies and registers matching types into a service registry. Separated from <see cref="DMContainer"/> to keep the DI facade focused on registration and resolution, not type scanning.</para>
    /// \endif
    /// </summary>
    public static class DreamineAutoRegistrar
    {
        /// <summary>
        /// \if KO
        /// <para>Service 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the service value.</para>
        /// \endif
        /// </summary>
        private static readonly AutoRegistrationService Service = new(
            new AssemblyTypeScanner(),
            new NamingConventionAutoRegistrationFilter());

        /// <summary>
        /// \if KO
        /// <para>Register All 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Scans <paramref name="rootAssembly"/> and all transitively referenced Dreamine assemblies for types that match the naming convention, then registers them into <paramref name="registry"/> with singleton lifetime.</para>
        /// \endif
        /// </summary>
        /// <param name="rootAssembly">
        /// \if KO
        /// <para>root Assembly에 사용할 <see cref="Assembly"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The assembly to scan first.</para>
        /// \endif
        /// </param>
        /// <param name="registry">
        /// \if KO
        /// <para>registry에 사용할 <see cref="IServiceRegistry"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The registry to register discovered types into.</para>
        /// \endif
        /// </param>
        public static void RegisterAll(Assembly rootAssembly, IServiceRegistry registry)
        {
            Service.RegisterAll(rootAssembly, registry);
        }

        /// <summary>
        /// \if KO
        /// <para>Register All 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Scans <paramref name="rootAssembly"/> and registers matching types into the global <see cref="DMContainer"/> singleton.</para>
        /// \endif
        /// </summary>
        /// <param name="rootAssembly">
        /// \if KO
        /// <para>root Assembly에 사용할 <see cref="Assembly"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The assembly to scan first.</para>
        /// \endif
        /// </param>
        public static void RegisterAll(Assembly rootAssembly)
        {
            Service.RegisterAll(rootAssembly, DMContainer.GetRegistry());
        }
    }
}
