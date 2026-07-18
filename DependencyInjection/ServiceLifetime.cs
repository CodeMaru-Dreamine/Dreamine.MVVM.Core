namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// \if KO
    /// <para>Service Lifetime 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Represents the lifetime of a registered service.</para>
    /// \endif
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// \if KO
        /// <para>Transient 값을 나타냅니다.</para>
        /// \endif
        /// \if EN
        /// <para>A new instance is created every time the service is resolved.</para>
        /// \endif
        /// </summary>
        Transient,

        /// <summary>
        /// \if KO
        /// <para>Singleton 값을 나타냅니다.</para>
        /// \endif
        /// \if EN
        /// <para>A single instance is reused for the lifetime of the container.</para>
        /// \endif
        /// </summary>
        Singleton
    }
}