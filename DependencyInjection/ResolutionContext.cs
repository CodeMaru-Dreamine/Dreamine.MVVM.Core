namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// \if KO
    /// <para>Resolution Context 기능과 관련 상태를 캡슐화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Tracks a single Resolve call graph so circular dependency detection is isolated per thread/async flow.</para>
    /// \endif
    /// </summary>
    internal sealed class ResolutionContext
    {
        /// <summary>
        /// \if KO
        /// <para>resolving Types 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the resolving types value.</para>
        /// \endif
        /// </summary>
        private readonly HashSet<Type> _resolvingTypes = new();

        /// <summary>
        /// \if KO
        /// <para>Enter 작업을 시도하고 성공 여부를 반환합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Attempts to enter and returns whether the operation succeeds.</para>
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
        /// <returns>
        /// \if KO
        /// <para>Try Enter 조건이 충족되면 <see langword="true"/>이고, 그렇지 않으면 <see langword="false"/>입니다.</para>
        /// \endif
        /// \if EN
        /// <para><see langword="true"/> when the try enter condition is satisfied; otherwise, <see langword="false"/>.</para>
        /// \endif
        /// </returns>
        public bool TryEnter(Type implementationType)
        {
            return _resolvingTypes.Add(implementationType);
        }

        /// <summary>
        /// \if KO
        /// <para>Exit 작업을 수행합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Performs the exit operation.</para>
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
        public void Exit(Type implementationType)
        {
            _resolvingTypes.Remove(implementationType);
        }
    }
}
