using System.Diagnostics;

namespace Furion.SchedulerSamples;

public interface ITestTransientService
{
    void Test();
}

public class TestTransientService : ITestTransientService, IDisposable
{
    public void Test()
    {
        Trace.WriteLine("call TestTransientService");
    }

    public void Dispose()
    {
        Trace.WriteLine("call TestTransientService.Dispose()");
    }
}