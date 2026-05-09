using System;
using Dreamine.MVVM.Interfaces.Locators;

namespace Dreamine.MVVM.Core.Locators;

/// <summary>
/// Resolves ViewModel instances through the Dreamine dependency container.
/// </summary>
public sealed class DreamineContainerViewModelResolver : IViewModelResolver
{
    /// <inheritdoc />
    public object? Resolve(Type viewModelType)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);

        return DMContainer.Resolve(viewModelType);
    }
}