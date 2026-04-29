namespace Dreamine.MVVM.Core.DependencyInjection
{
    /// <summary>
    /// Represents the lifetime of a registered service.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// A new instance is created every time the service is resolved.
        /// </summary>
        Transient,

        /// <summary>
        /// A single instance is reused for the lifetime of the container.
        /// </summary>
        Singleton
    }
}