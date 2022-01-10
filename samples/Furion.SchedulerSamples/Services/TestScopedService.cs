namespace Furion.SchedulerSamples;

public interface ITestScopedService
{
    void Test();
}

public class TestScopedService : ITestScopedService, IDisposable
{
    public void Test()
    {
        Console.WriteLine("call TestScopedService");
    }

    public void Dispose()
    {
        Console.WriteLine("call TestScopedService.Dispose()");
    }
}