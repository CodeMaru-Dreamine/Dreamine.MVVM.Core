using System;
using Dreamine.MVVM.Interfaces.Locators;

namespace Dreamine.MVVM.Core.Locators;

/// <summary>
/// \if KO
/// <para>Dreamine Container View Model Resolver 기능과 관련 상태를 캡슐화합니다.</para>
/// \endif
/// \if EN
/// <para>Resolves ViewModel instances through the Dreamine dependency container.</para>
/// \endif
/// </summary>
public sealed class DreamineContainerViewModelResolver : IViewModelResolver
{
    /// <summary>
    /// \if KO
    /// <para>Resolve 작업을 수행합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Performs the resolve operation.</para>
    /// \endif
    /// </summary>
    /// <param name="viewModelType">
    /// \if KO
    /// <para>view Model Type에 사용할 <see cref="Type"/> 값입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The <see cref="Type"/> value used for view model type.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>Resolve 작업에서 생성한 <see cref="object"/> 결과입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The <see cref="object"/> result produced by the resolve operation.</para>
    /// \endif
    /// </returns>
    public object? Resolve(Type viewModelType)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);

        return DMContainer.Resolve(viewModelType);
    }
}