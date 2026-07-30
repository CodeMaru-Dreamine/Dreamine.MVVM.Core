using System.Reflection;
using Dreamine.MVVM.Core.AutoRegistration;
using Dreamine.MVVM.Core.DependencyInjection;
using Dreamine.MVVM.Interfaces.DependencyInjection;
using Xunit;

namespace Dreamine.MVVM.Core.Tests;

public sealed class AutoRegistrationTests
{
    [Theory]
    [InlineData(typeof(CustomerModel), true)]
    [InlineData(typeof(CustomerEvent), true)]
    [InlineData(typeof(CustomerViewModel), true)]
    [InlineData(typeof(CustomerManager), false)]
    [InlineData(typeof(ExplicitUtility), true)]
    public void NamingConventionRecognizesSupportedTypes(Type type, bool expected)
    {
        Assert.Equal(expected, new NamingConventionAutoRegistrationFilter().IsTarget(type));
    }

    [Fact]
    public void ServiceRegistersMatchingTypesAsSingletons()
    {
        using var container = new DreamineContainer();
        var scanner = new TestScanner(typeof(CustomerModel).Assembly);
        var service = new AutoRegistrationService(scanner, new NamingConventionAutoRegistrationFilter());
        service.RegisterAll(typeof(CustomerModel).Assembly, container);
        Assert.True(container.IsRegistered(typeof(CustomerModel)));
        Assert.Same(container.Resolve<CustomerModel>(), container.Resolve<CustomerModel>());
        Assert.False(container.IsRegistered(typeof(CustomerManager)));
    }

    private sealed class TestScanner(Assembly assembly) : IAssemblyTypeScanner
    {
        public IEnumerable<Assembly> GetCandidateAssemblies(Assembly rootAssembly)
        {
            yield return assembly;
        }
        public IEnumerable<Type> GetLoadableTypes(Assembly target) => target.GetTypes();
    }

    public sealed class CustomerModel;
    public sealed class CustomerEvent;
    public sealed class CustomerViewModel;
    public sealed class CustomerManager;
    [DreamineRegister]
    public sealed class ExplicitUtility;

    [AttributeUsage(AttributeTargets.Class)]
    private sealed class DreamineRegisterAttribute : Attribute;
}
