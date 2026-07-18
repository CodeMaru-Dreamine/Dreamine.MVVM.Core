using System;

namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// \if KO
    /// <para>Service Descriptor 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Describes a registered service.</para>
    /// \endif
    /// </summary>
    public sealed class ServiceDescriptor
    {
        /// <summary>
        /// \if KO
        /// <para>지정한 설정으로 <see cref="ServiceDescriptor"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes a new instance of the <see cref="ServiceDescriptor"/> class.</para>
        /// \endif
        /// </summary>
        /// <param name="serviceType">
        /// \if KO
        /// <para>service Type에 사용할 <see cref="Type"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service abstraction type.</para>
        /// \endif
        /// </param>
        /// <param name="implementationType">
        /// \if KO
        /// <para>implementation Type에 사용할 <see cref="Type"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The implementation type.</para>
        /// \endif
        /// </param>
        /// <param name="factory">
        /// \if KO
        /// <para>factory에 사용할 <c>Func&lt;object&gt;</c> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The optional factory.</para>
        /// \endif
        /// </param>
        /// <param name="instance">
        /// \if KO
        /// <para>instance에 사용할 <see cref="object"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The optional singleton instance.</para>
        /// \endif
        /// </param>
        /// <param name="lifetime">
        /// \if KO
        /// <para>lifetime에 사용할 <see cref="ServiceLifetime"/> 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service lifetime.</para>
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
        public ServiceDescriptor(
            Type serviceType,
            Type? implementationType,
            Func<object>? factory,
            object? instance,
            ServiceLifetime lifetime)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            ImplementationType = implementationType;
            Factory = factory;
            Instance = instance;
            Lifetime = lifetime;
        }

        /// <summary>
        /// \if KO
        /// <para>Service Type 값을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the service abstraction type.</para>
        /// \endif
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// \if KO
        /// <para>Implementation Type 값을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the concrete implementation type.</para>
        /// \endif
        /// </summary>
        public Type? ImplementationType { get; }

        /// <summary>
        /// \if KO
        /// <para>Factory 값을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the factory used to create the service instance.</para>
        /// \endif
        /// </summary>
        public Func<object>? Factory { get; }

        /// <summary>
        /// \if KO
        /// <para>Instance 값을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the registered singleton instance.</para>
        /// \endif
        /// </summary>
        public object? Instance { get; }

        /// <summary>
        /// \if KO
        /// <para>Lifetime 값을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the service lifetime.</para>
        /// \endif
        /// </summary>
        public ServiceLifetime Lifetime { get; }
    }
}