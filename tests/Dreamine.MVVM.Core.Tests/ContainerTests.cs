using Dreamine.MVVM.Core;
using Dreamine.MVVM.Core.AutoRegistration;
using Dreamine.MVVM.Core.DependencyInjection;
using Dreamine.MVVM.Core.Locators;
using Xunit;

namespace Dreamine.MVVM.Core.Tests;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ContainerCollection : ICollectionFixture<ContainerFixture>
{
    public const string Name = "DMContainer";
}

public sealed class ContainerFixture : IDisposable
{
    public ContainerFixture() => DMContainer.Reset();
    public void Dispose() => DMContainer.Reset();
}

[Collection(ContainerCollection.Name)]
public sealed class ContainerTests
{
    [Fact]
    public void TransientResolutionInjectsDependenciesAndCreatesNewInstances()
    {
        using var container = new DreamineContainer();
        container.Register<IClock, FixedClock>();
        container.Register<ReportService>();

        var first = container.Resolve<ReportService>();
        var second = container.Resolve<ReportService>();

        Assert.NotSame(first, second);
        Assert.Equal("2026-06-07", first.CreateReportName());
    }

    [Fact]
    public void SingletonResolutionReusesOneInstance()
    {
        using var container = new DreamineContainer();
        container.RegisterSingleton<IClock, FixedClock>();
        Assert.Same(container.Resolve<IClock>(), container.Resolve<IClock>());
    }

    [Fact]
    public void FactoryRegistrationCreatesTransientValues()
    {
        using var container = new DreamineContainer();
        container.Register(() => new Token(Guid.NewGuid()));
        Assert.NotEqual(container.Resolve<Token>().Value, container.Resolve<Token>().Value);
    }

    [Fact]
    public void RegisteredInstanceIsReturned()
    {
        using var container = new DreamineContainer();
        var instance = new FixedClock();
        container.RegisterSingleton<IClock>(instance);
        Assert.Same(instance, container.Resolve<IClock>());
    }

    [Fact]
    public void UnregisteredConcreteTypeCanBeCreated()
    {
        using var container = new DreamineContainer();
        Assert.IsType<FixedClock>(container.Resolve<FixedClock>());
    }

    [Fact]
    public void UnregisteredInterfaceCannotBeResolved()
    {
        using var container = new DreamineContainer();
        Assert.False(container.TryResolve<IClock>(out var result));
        Assert.Null(result);
        Assert.Throws<InvalidOperationException>(() => container.Resolve<IClock>());
    }

    [Fact]
    public void CircularDependencyIsRejected()
    {
        using var container = new DreamineContainer();
        container.Register<CircularA>();
        container.Register<CircularB>();
        var error = Assert.Throws<InvalidOperationException>(() => container.Resolve<CircularA>());
        Assert.Contains("Circular dependency", error.Message);
    }

    [Fact]
    public async Task SingletonCreationIsThreadSafe()
    {
        SlowSingleton.Reset();
        using var container = new DreamineContainer();
        container.RegisterSingleton<SlowSingleton>();
        var values = await Task.WhenAll(Enumerable.Range(0, 32)
            .Select(_ => Task.Run(container.Resolve<SlowSingleton>)));
        Assert.Single(values.Distinct());
        Assert.Equal(1, SlowSingleton.CreatedCount);
    }

    [Fact]
    public void ReRegistrationReplacesCachedSingleton()
    {
        using var container = new DreamineContainer();
        container.RegisterSingleton<IClock, FixedClock>();
        _ = container.Resolve<IClock>();
        container.Register<IClock, AlternateClock>();
        Assert.IsType<AlternateClock>(container.Resolve<IClock>());
    }

    [Fact]
    public void DisposeDisposesSingletonOnce()
    {
        var container = new DreamineContainer();
        var disposable = new DisposableService();
        container.RegisterSingleton(disposable);
        container.Dispose();
        container.Dispose();
        Assert.Equal(1, disposable.DisposeCount);
    }

    [Fact]
    public void StaticFacadeCanRegisterResolveAndReset()
    {
        DMContainer.Reset();
        DMContainer.Register<IClock, FixedClock>();
        Assert.True(DMContainer.IsRegistered<IClock>());
        Assert.IsType<FixedClock>(DMContainer.GetResolver().Resolve<IClock>());
        DMContainer.Reset();
        Assert.False(DMContainer.IsRegistered<IClock>());
    }

    [Fact]
    public void StaticFacadeSupportsEveryRegistrationShape()
    {
        DMContainer.Register<FixedClock>();
        Assert.IsType<FixedClock>(DMContainer.Resolve<FixedClock>());

        DMContainer.Register(() => new Token(Guid.NewGuid()));
        Assert.NotEqual(DMContainer.Resolve<Token>().Value, DMContainer.Resolve<Token>().Value);

        var instance = new AlternateClock();
        DMContainer.RegisterSingleton<IClock>(instance);
        Assert.Same(instance, DMContainer.Resolve<IClock>());
        Assert.True(DMContainer.IsRegistered(typeof(IClock)));

        DMContainer.RegisterSingleton<SlowSingleton>();
        Assert.Same(DMContainer.Resolve<SlowSingleton>(), DMContainer.Resolve(typeof(SlowSingleton)));

        DMContainer.RegisterSingleton<IClock, FixedClock>();
        Assert.IsType<FixedClock>(DMContainer.Resolve<IClock>());
        Assert.True(DMContainer.TryResolve<IClock>(out var resolved));
        Assert.NotNull(resolved);
        Assert.False(DMContainer.TryResolve<IUnregistered>(out _));
    }

    [Fact]
    public void StaticFacadeCanReplaceItsContainer()
    {
        using var replacement = new DreamineContainer();
        replacement.RegisterSingleton<IClock>(new AlternateClock());

        DMContainer.SetContainer(replacement);

        Assert.Same(replacement, DMContainer.GetResolver());
        Assert.IsType<AlternateClock>(DMContainer.Resolve<IClock>());
        Assert.Throws<ArgumentNullException>(() => DMContainer.SetContainer(null!));
        Assert.Throws<ArgumentNullException>(() => DMContainer.Resolve(null!));
        Assert.Throws<ArgumentNullException>(() => DMContainer.IsRegistered(null!));
    }

    [Fact]
    public void ScannerReturnsRootAssemblyAndItsLoadableTypes()
    {
        var scanner = new AssemblyTypeScanner();
        var assembly = typeof(ContainerTests).Assembly;

        Assert.Contains(assembly, scanner.GetCandidateAssemblies(assembly));
        Assert.Contains(typeof(ContainerTests), scanner.GetLoadableTypes(assembly));
        Assert.Throws<ArgumentNullException>(() => scanner.GetCandidateAssemblies(null!).ToArray());
        Assert.Throws<ArgumentNullException>(() => scanner.GetLoadableTypes(null!).ToArray());
    }

    [Fact]
    public void ViewModelResolverUsesTheStaticContainer()
    {
        DMContainer.Register<FixedClock>();
        var resolver = new DreamineContainerViewModelResolver();

        Assert.IsType<FixedClock>(resolver.Resolve(typeof(FixedClock)));
        Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null!));
    }

    [Fact]
    public void ConstructorSelectorRejectsInvalidTypesAndSelectsGreediestConstructor()
    {
        var selector = new ConstructorSelector();

        Assert.Equal(2, selector.SelectConstructor(typeof(MultipleConstructors)).GetParameters().Length);
        Assert.Throws<ArgumentNullException>(() => selector.SelectConstructor(null!));
        Assert.Throws<InvalidOperationException>(() => selector.SelectConstructor(typeof(NoPublicConstructor)));
    }

    private interface IClock { DateOnly Today { get; } }
    private interface IUnregistered { }
    private sealed class FixedClock : IClock { public DateOnly Today => new(2026, 6, 7); }
    private sealed class AlternateClock : IClock { public DateOnly Today => new(2026, 6, 8); }
    private sealed record Token(Guid Value);
    private sealed class ReportService(IClock clock)
    {
        public string CreateReportName() => clock.Today.ToString("yyyy-MM-dd");
    }
    private sealed class CircularA(CircularB dependency) { public CircularB Dependency { get; } = dependency; }
    private sealed class CircularB(CircularA dependency) { public CircularA Dependency { get; } = dependency; }
    private sealed class SlowSingleton
    {
        private static int _count;
        public SlowSingleton() { Thread.Sleep(10); Interlocked.Increment(ref _count); }
        public static int CreatedCount => Volatile.Read(ref _count);
        public static void Reset() => Volatile.Write(ref _count, 0);
    }
    private sealed class DisposableService : IDisposable
    {
        public int DisposeCount { get; private set; }
        public void Dispose() => DisposeCount++;
    }
    private sealed class MultipleConstructors
    {
        public MultipleConstructors() { }
        public MultipleConstructors(IClock clock, Token token) { }
    }
    private sealed class NoPublicConstructor
    {
        private NoPublicConstructor() { }
    }
}
