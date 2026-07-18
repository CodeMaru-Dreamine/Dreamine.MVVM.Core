using System;
using System.Linq;
using Dreamine.MVVM.Interfaces.DependencyInjection;

namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// \if KO
    /// <para>Constructor Activator 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates object instances by resolving constructor parameters.</para>
    /// \endif
    /// </summary>
    public sealed class ConstructorActivator : IObjectActivator
    {
        /// <summary>
        /// \if KO
        /// <para>constructor Selector 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the constructor selector value.</para>
        /// \endif
        /// </summary>
        private readonly IConstructorSelector _constructorSelector;

        /// <summary>
        /// \if KO
        /// <para>지정한 설정으로 <see cref="ConstructorActivator"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes a new instance of the <see cref="ConstructorActivator"/> class.</para>
        /// \endif
        /// </summary>
        /// <param name="constructorSelector">
        /// \if KO
        /// <para>constructor Selector에 사용할 <see cref="IConstructorSelector"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The constructor selector.</para>
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
        public ConstructorActivator(IConstructorSelector constructorSelector)
        {
            _constructorSelector = constructorSelector
                ?? throw new ArgumentNullException(nameof(constructorSelector));
        }

        /// <summary>
        /// \if KO
        /// <para>Instance 값을 생성합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Creates an instance of the specified implementation type.</para>
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
        /// <param name="resolver">
        /// \if KO
        /// <para>resolver에 사용할 <see cref="IServiceResolver"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The resolver used to resolve constructor dependencies.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>Create Instance 작업에서 생성한 <see cref="object"/> 결과입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The created object instance.</para>
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
        /// <para>현재 객체 상태에서 Create Instance 작업을 수행할 수 없는 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when the create instance operation is not valid for the current object state.</para>
        /// \endif
        /// </exception>
        public object CreateInstance(Type implementationType, IServiceResolver resolver)
        {
            if (implementationType is null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            var constructor = _constructorSelector.SelectConstructor(implementationType);

            object[] arguments = constructor
                .GetParameters()
                .Select(parameter => resolver.Resolve(parameter.ParameterType))
                .ToArray();

            return Activator.CreateInstance(implementationType, arguments)
                ?? throw new InvalidOperationException(
                    $"Failed to create instance of [{implementationType.FullName}].");
        }
    }
}