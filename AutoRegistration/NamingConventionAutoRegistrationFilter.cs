using System;
using System.Linq;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.AutoRegistration
{
    /// <summary>
    /// \if KO
    /// <para>Naming Convention Auto Registration Filter 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Determines whether a type matches Dreamine auto-registration naming conventions.</para>
    /// \endif
    /// </summary>
    public sealed class NamingConventionAutoRegistrationFilter : IAutoRegistrationFilter
    {
        /// <summary>
        /// \if KO
        /// <para>Is Target 조건을 확인합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Determines whether the specified type is eligible for auto registration.</para>
        /// \endif
        /// </summary>
        /// <param name="type">
        /// \if KO
        /// <para>type에 사용할 <see cref="Type"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The type to inspect.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Is Target 조건이 충족되면 <see langword="true"/>이고, 그렇지 않으면 <see langword="false"/>입니다.</para>
        /// \endif
        /// \if EN
        /// <para>True if the type is eligible; otherwise false.</para>
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

            return HasExplicitDreamineRegistrationAttribute(type) ||
                   name.EndsWith("Model", StringComparison.Ordinal) ||
                   name.EndsWith("Event", StringComparison.Ordinal) ||
                   IsManagerTarget(name, fullName) ||
                   name.EndsWith("ViewModel", StringComparison.Ordinal) ||
                   fullName.Contains(".xaml.ViewModel", StringComparison.Ordinal) ||
                   fullName.Contains(".xaml.Model", StringComparison.Ordinal) ||
                   fullName.Contains(".xaml.Event", StringComparison.Ordinal);
        }

        /// <summary>
        /// \if KO
        /// <para>Is Concrete Class 조건을 확인합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Determines whether is concrete class.</para>
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
        /// <returns>
        /// \if KO
        /// <para>Is Concrete Class 조건이 충족되면 <see langword="true"/>이고, 그렇지 않으면 <see langword="false"/>입니다.</para>
        /// \endif
        /// \if EN
        /// <para><see langword="true"/> when the is concrete class condition is satisfied; otherwise, <see langword="false"/>.</para>
        /// \endif
        /// </returns>
        private static bool IsConcreteClass(Type type)
        {
            return type.IsClass &&
                   !type.IsAbstract &&
                   !type.IsGenericTypeDefinition;
        }

        /// <summary>
        /// \if KO
        /// <para>Is Manager Target 조건을 확인합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Determines whether is manager target.</para>
        /// \endif
        /// </summary>
        /// <param name="name">
        /// \if KO
        /// <para>name에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for name.</para>
        /// \endif
        /// </param>
        /// <param name="fullName">
        /// \if KO
        /// <para>full Name에 사용할 <see cref="string"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The <see cref="string"/> value used for full name.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Is Manager Target 조건이 충족되면 <see langword="true"/>이고, 그렇지 않으면 <see langword="false"/>입니다.</para>
        /// \endif
        /// \if EN
        /// <para><see langword="true"/> when the is manager target condition is satisfied; otherwise, <see langword="false"/>.</para>
        /// \endif
        /// </returns>
        private static bool IsManagerTarget(string name, string fullName)
        {
            return name.EndsWith("Manager", StringComparison.Ordinal) &&
                   (fullName.Contains(".Managers.", StringComparison.Ordinal) ||
                    fullName.Contains(".xaml.", StringComparison.Ordinal));
        }

        /// <summary>
        /// \if KO
        /// <para>Has Explicit Dreamine Registration Attribute 조건을 확인합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Determines whether has explicit dreamine registration attribute.</para>
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
        /// <returns>
        /// \if KO
        /// <para>Has Explicit Dreamine Registration Attribute 조건이 충족되면 <see langword="true"/>이고, 그렇지 않으면 <see langword="false"/>입니다.</para>
        /// \endif
        /// \if EN
        /// <para><see langword="true"/> when the has explicit dreamine registration attribute condition is satisfied; otherwise, <see langword="false"/>.</para>
        /// \endif
        /// </returns>
        private static bool HasExplicitDreamineRegistrationAttribute(Type type)
        {
            return type.GetCustomAttributes(inherit: false)
                .Any(attribute => attribute.GetType().Name is "DreamineRegisterAttribute" or "DreamineAutoRegisterAttribute");
        }
    }
}
