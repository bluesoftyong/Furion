
namespace Furion.TestProject.Services;

public interface ITestDependencyService
{
}

public class TestDependencyService : ITestDependencyService, ITransientService
{
}
public interface ITestGenericDependencyService<T>
{
}

public class TestGenericDependencyService<T> : ITestGenericDependencyService<T>, ITransientService
{
}


public interface ITest3DependencyService
{
    bool NoNull();
}

public class Test3DependencyService : ITest3DependencyService, IFactoryService<ITransientService>
{
    public Test3DependencyService()
    {
    }

    [AutowiredServices]
    IApp? App { get; }

    public bool NoNull()
    {
        return App != null;
    }

    public object ServiceFactory(IServiceProvider serviceProvider)
    {
        return new Test3DependencyService();
    }
}