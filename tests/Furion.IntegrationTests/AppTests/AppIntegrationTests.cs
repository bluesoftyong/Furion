using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Furion.IntegrationTests;

/// <summary>
/// App 模块集成测试
/// </summary>
public class AppIntegrationTests : IClassFixture<WebApplicationFactory<Furion.AppSamples.FakeStartup>>
{
    private readonly WebApplicationFactory<Furion.AppSamples.FakeStartup> _factory;
    public AppIntegrationTests(WebApplicationFactory<Furion.AppSamples.FakeStartup> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// 测试 App 模块示例可用性
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/api/IApp/GetConfiguration")]
    [InlineData("/api/IApp/GetEnvironmentInfo")]
    [InlineData("/api/IApp/GetServiceByHostServices")]
    [InlineData("/api/IApp/GetServiceByServiceProvider")]
    [InlineData("/api/IApp/GetService")]
    [InlineData("/api/IApp/GetRequiredService")]
    [InlineData("/api/AppSettingsOptions/GetAppSettings")]
    public async Task TestEnsureSuccessStatusCode(string url)
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }
}