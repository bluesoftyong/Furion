using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Furion.IntegrationTests;

/// <summary>
/// App 模块集成测试
/// </summary>
public class AppTests : IClassFixture<WebApplicationFactory<AppTestProject.FakeStartup>>
{
    private readonly ITestOutputHelper _output;
    private readonly WebApplicationFactory<AppTestProject.FakeStartup> _factory;

    public AppTests(ITestOutputHelper output,
        WebApplicationFactory<AppTestProject.FakeStartup> factory)
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
    [InlineData("/TestIApp/TestIAppIsSingleton")]
    public async Task TestIAppIsSingleton(string url)
    {
        await _factory.PostAsync(url);
    }

    /// <summary>
    /// 测试 IApp 实例
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/TestIApp/TestIAppNotPublicInstance")]
    public async Task TestIAppNotPublicInstance(string url)
    {
        await _factory.PostAsync(url);
    }

    /// <summary>
    /// 测试 IApp 属性
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/TestIApp/TestIAppProperties")]
    public async Task TestIAppProperties(string url)
    {
        await _factory.PostAsync(url);
    }

    /// <summary>
    /// 测试 IApp 方法
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/TestIApp/TestIAppMethods")]
    public async Task TestIAppMethods(string url)
    {
        await _factory.PostAsync(url);
    }
}