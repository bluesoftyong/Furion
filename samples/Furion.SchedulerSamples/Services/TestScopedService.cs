using System.Diagnostics;

namespace Furion.SchedulerSamples;

public interface ITestScopedService
{
    void Test();
}

public class TestScopedService : ITestScopedService, IDisposable
{
    public void Test()
    {
        Trace.WriteLine("call TestScopedService");
    }

    public void Dispose()
    {
        Trace.WriteLine("call TestScopedService.Dispose()");
    }
}