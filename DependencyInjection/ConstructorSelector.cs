using System;
using System.Linq;
using System.Reflection;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// \if KO
    /// <para>Constructor Selector 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Selects the constructor with the largest number of parameters.</para>
    /// \endif
    /// </summary>
    public sealed class ConstructorSelector : IConstructorSelector
    {
        /// <summary>
        /// \if KO
        /// <para>Select Constructor 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Selects the constructor to use for the specified implementation type.</para>
        /// \endif
        /// </summary>
        /// <param name="implementationType">
        /// \if KO
        /// <para>implementation Type에 사용할 <see cref="Type"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The implementation type.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Select Constructor 작업에서 생성한 <see cref="ConstructorInfo"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The selected constructor.</para>
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
        /// <exception cref="InvalidOperationException">
        /// \if KO
        /// <para>현재 객체 상태에서 Select Constructor 작업을 수행할 수 없는 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when the select constructor operation is not valid for the current object state.</para>
        /// \endif
        /// </exception>
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