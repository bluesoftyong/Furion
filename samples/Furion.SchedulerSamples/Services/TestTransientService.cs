namespace Furion.SchedulerSamples;

public interface ITestTransientService
{
    void Test();
}

public class TestTransientService : ITestTransientService, IDisposable
{
    public void Test()
    {
        Console.WriteLine("call TestTransientService");
    }

    public void Dispose()
    {
        Console.WriteLine("call TestTransientService.Dispose()");
    }
}