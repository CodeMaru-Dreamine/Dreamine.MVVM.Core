using System;
using System.Linq;
using System.Reflection;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.AutoRegistration
{
    /// <summary>
    /// \if KO
    /// <para>Auto Registration Service 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Automatically registers Dreamine-supported types.</para>
    /// \endif
    /// </summary>
    public sealed class AutoRegistrationService : IAutoRegistrationService
    {
        /// <summary>
        /// \if KO
        /// <para>type Scanner 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the type scanner value.</para>
        /// \endif
        /// </summary>
        private readonly IAssemblyTypeScanner _typeScanner;
        /// <summary>
        /// \if KO
        /// <para>filter 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the filter value.</para>
        /// \endif
        /// </summary>
        private readonly IAutoRegistrationFilter _filter;

        /// <summary>
        /// \if KO
        /// <para>지정한 설정으로 <see cref="AutoRegistrationService"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes a new instance of the <see cref="AutoRegistrationService"/> class.</para>
        /// \endif
        /// </summary>
        /// <param name="typeScanner">
        /// \if KO
        /// <para>type Scanner에 사용할 <see cref="IAssemblyTypeScanner"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The assembly type scanner.</para>
        /// \endif
        /// </param>
        /// <param name="filter">
        /// \if KO
        /// <para>filter에 사용할 <see cref="IAutoRegistrationFilter"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The auto-registration filter.</para>
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
        public AutoRegistrationService(
            IAssemblyTypeScanner typeScanner,
            IAutoRegistrationFilter filter)
        {
            _typeScanner = typeScanner ?? throw new ArgumentNullException(nameof(typeScanner));
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        /// <summary>
        /// \if KO
        /// <para>Register All 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers supported types from the specified root assembly.</para>
        /// \endif
        /// </summary>
        /// <param name="rootAssembly">
        /// \if KO
        /// <para>root Assembly에 사용할 <see cref="Assembly"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The root assembly to scan first.</para>
        /// \endif
        /// </param>
        /// <param name="registry">
        /// \if KO
        /// <para>registry에 사용할 <see cref="IServiceRegistry"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service registry to populate.</para>
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

        /// <summary>
        /// \if KO
        /// <para>Register Assembly Types 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the register assembly types operation.</para>
        /// \endif
        /// </summary>
        /// <param name="assembly">
        /// \if KO
        /// <para>assembly에 사용할 <see cref="Assembly"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="Assembly"/> value used for assembly.</para>
        /// \endif
        /// </param>
        /// <param name="registry">
        /// \if KO
        /// <para>registry에 사용할 <see cref="IServiceRegistry"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="IServiceRegistry"/> value used for registry.</para>
        /// \endif
        /// </param>
        private void RegisterAssemblyTypes(Assembly assembly, IServiceRegistry registry)
        {
            foreach (Type type in _typeScanner.GetLoadableTypes(assembly))
            {
                RegisterType(type, registry);
            }
        }

        /// <summary>
        /// \if KO
        /// <para>Register Type 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the register type operation.</para>
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
        /// <param name="registry">
        /// \if KO
        /// <para>registry에 사용할 <see cref="IServiceRegistry"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="IServiceRegistry"/> value used for registry.</para>
        /// \endif
        /// </param>
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

        /// <summary>
        /// \if KO
        /// <para>Register Singleton By Reflection 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the register singleton by reflection operation.</para>
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
        /// <param name="registry">
        /// \if KO
        /// <para>registry에 사용할 <see cref="IServiceRegistry"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="IServiceRegistry"/> value used for registry.</para>
        /// \endif
        /// </param>
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