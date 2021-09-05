using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace Furion.IntegrationTests;

/// <summary>
/// 依赖注入集成测试
/// </summary>
public class DependencyInjectionTests : IClassFixture<WebApplicationFactory<TestProject.FakeStartup>>
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
    public DependencyInjectionTests(ITestOutputHelper output,
        WebApplicationFactory<TestProject.FakeStartup> factory)
    {
        _output = output;
        _factory = factory;
    }

    /// <summary>
    /// 测试自定义服务提供器
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/DependencyInjectionTests/TestService")]
    public async Task TestService(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        Assert.True(bool.Parse(content));
    }

    /// <summary>
    /// 测试命名服务
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/DependencyInjectionTests/TestNamedService")]
    public async Task TestNamedService(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        var result = JsonSerializer.Deserialize<string[]>(content);

        Assert.Equal("Test2NamedService", result!.Last());
    }

    /// <summary>
    /// 测试扫描依赖注入服务
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/DependencyInjectionTests/TestDependencyService")]
    public async Task TestDependencyService(string url)
    {
        var content = await _factory.PostAsStringAsync(url, ensure: false);
        _output.WriteLine($"{content}");

        Assert.True(bool.Parse(content));
    }

    /// <summary>
    /// 测试泛型接口
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/DependencyInjectionTests/TestGenericDependencyService")]
    public async Task TestGenericDependencyService(string url)
    {
        var content = await _factory.PostAsStringAsync(url, ensure: false);
        _output.WriteLine($"{content}");

        Assert.True(bool.Parse(content));
    }

    /// <summary>
    /// 测试工厂解析服务
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/DependencyInjectionTests/TestFactoryService")]
    public async Task TestFactoryService(string url)
    {
        var content = await _factory.PostAsStringAsync(url, ensure: false);
        _output.WriteLine($"{content}");

        Assert.True(bool.Parse(content));
    }
}