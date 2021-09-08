using Microsoft.Extensions.DependencyInjection;
using System;

namespace Furion.TestWorkerProject.Services;

public interface ITestService
{
    DateTimeOffset GetDateTime();
}

public class TestService : ITestService, ITransientService
{
    [AutowiredServices]
    IApp? App { get; }

    public DateTimeOffset GetDateTime()
    {
        if (App == null) throw new Exception("为空");

        return DateTimeOffset.Now;
    }
}