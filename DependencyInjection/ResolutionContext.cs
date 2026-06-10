namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// Tracks a single Resolve call graph so circular dependency detection is isolated per thread/async flow.
    /// </summary>
    internal sealed class ResolutionContext
    {
        private readonly HashSet<Type> _resolvingTypes = new();

        public bool TryEnter(Type implementationType)
        {
            return _resolvingTypes.Add(implementationType);
        }

        public void Exit(Type implementationType)
        {
            _resolvingTypes.Remove(implementationType);
        }
    }
}
