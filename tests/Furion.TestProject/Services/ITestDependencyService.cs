
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