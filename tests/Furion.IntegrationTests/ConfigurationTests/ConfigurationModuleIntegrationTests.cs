using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Furion.IntegrationTests;

/// <summary>
/// Configuration 模块集成测试
/// </summary>
public class ConfigurationModuleIntegrationTests : IClassFixture<WebApplicationFactory<Furion.ConfigurationSamples.FakeStartup>>
{
    private readonly WebApplicationFactory<Furion.ConfigurationSamples.FakeStartup> _factory;
    public ConfigurationModuleIntegrationTests(WebApplicationFactory<Furion.ConfigurationSamples.FakeStartup> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// 测试 Configuration 模块示例可用性
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/api/Configuration/GetString")]
    [InlineData("/api/Configuration/GetBoolean")]
    [InlineData("/api/Configuration/GetBoolean2")]
    [InlineData("/api/Configuration/GetInt")]
    [InlineData("/api/Configuration/GetLong")]
    [InlineData("/api/Configuration/GetFloat")]
    [InlineData("/api/Configuration/GetDecimal")]
    [InlineData("/api/Configuration/GetEnum")]
    [InlineData("/api/Configuration/GetEnum2")]
    [InlineData("/api/Configuration/GetArray")]
    [InlineData("/api/Configuration/GetArray2")]
    [InlineData("/api/Configuration/GetDictionary")]
    [InlineData("/api/Configuration/GetObject")]

    [InlineData("/api/ConfigurationSource/GetEnvironmentValue")]
    [InlineData("/api/ConfigurationSource/GetCommandLineValue")]
    [InlineData("/api/ConfigurationSource/GetMemoryValue")]
    [InlineData("/api/ConfigurationSource/GetXmlValue")]
    [InlineData("/api/ConfigurationSource/GetIniValue")]
    [InlineData("/api/ConfigurationSource/GetKeyPerFileValue")]
    [InlineData("/api/ConfigurationSource/GetTxtValue")]
    public async Task TestEnsureSuccessStatusCode(string url)
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// 测试节点存在性
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/api/Configuration/CheckExists", "string")]
    [InlineData("/api/Configuration/CheckExists", "unknown")]
    public async Task TestCheckExists(string url, string key)
    {
        using var client = _factory.CreateClient();
        using var response = await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(key), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }
}