using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Furion.IntegrationTests;

/// <summary>
/// App 全局应用对象集成测试
/// </summary>
public class AppTests : IClassFixture<WebApplicationFactory<TestProject.FakeStartup>>
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
    public AppTests(ITestOutputHelper output,
        WebApplicationFactory<TestProject.FakeStartup> factory)
    {
        _output = output;
        _factory = factory;
    }

    /// <summary>
    /// 测试 IApp 是否是单例
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/AppTests/TestIsSingleton")]
    public async Task TestIsSingleton(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        Assert.True(bool.Parse(content));
    }

    /// <summary>
    /// 测试 AppSettings 不为 Null
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/AppTests/TestAppSettingsNotNull")]
    public async Task TestAppSettingsNotNull(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        Assert.True(bool.Parse(content));
    }
}