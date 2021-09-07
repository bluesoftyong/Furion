using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace Furion.UnitTests;

public class ConfigurationTests
{
    [Fact]
    public void Test1()
    {
        var host = WebHost.CreateDefaultBuilder()
               .Configure(_ => { })
               .Build();
    }
}