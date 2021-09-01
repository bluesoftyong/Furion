using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace Furion.IntegrationTests;

/// <summary>
///选项模块集成测试
/// </summary>
public class OptionsTests : IClassFixture<WebApplicationFactory<TestProject.FakeStartup>>
{
    /// <summary>
    /// 标准输出
    /// </summary>
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Web 应用工厂
    /// </summary>
    private readonly WebApplicationFactory<TestProject.FakeStartup> _factory;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="output">标准输出</param>
    /// <param name="factory">Web 应用工厂</param>
    public OptionsTests(ITestOutputHelper output,
        WebApplicationFactory<TestProject.FakeStartup> factory)
    {
        _output = output;
        _factory = factory;
    }

    /// <summary>
    /// 测试选项多种类型配置是否有效
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/OptionsTests/TestNotImplementOptions")]
    [InlineData("/OptionsTests/TestImplementOptions")]
    public async Task TestOptions(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        var result = JsonSerializer.Deserialize<string[]>(content);

        Assert.All(result, str =>
        {
            Assert.Equal("Furion!", str);
        });
    }
}