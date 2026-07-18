using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.AutoRegistration
{
    /// <summary>
    /// \if KO
    /// <para>Assembly Type Scanner 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Scans assemblies and returns loadable types safely.</para>
    /// \endif
    /// </summary>
    public sealed class AssemblyTypeScanner : IAssemblyTypeScanner
    {
        /// <summary>
        /// \if KO
        /// <para>Candidate Assemblies 값을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets candidate assemblies for auto registration.</para>
        /// \endif
        /// </summary>
        /// <param name="rootAssembly">
        /// \if KO
        /// <para>root Assembly에 사용할 <see cref="Assembly"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The root assembly to prioritize.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Get Candidate Assemblies 작업에서 생성한 <see cref="IEnumerable{Assembly}"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The candidate assemblies.</para>
        /// \endif
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para>필수 입력 인자 중 하나가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when a required input argument is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
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
        /// \if KO
        /// <para>Loadable Types 값을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets loadable types from the specified assembly.</para>
        /// \endif
        /// </summary>
        /// <param name="assembly">
        /// \if KO
        /// <para>assembly에 사용할 <see cref="Assembly"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The assembly to scan.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Get Loadable Types 작업에서 생성한 <see cref="IEnumerable{Type}"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The loadable types.</para>
        /// \endif
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para>필수 입력 인자 중 하나가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when a required input argument is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
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