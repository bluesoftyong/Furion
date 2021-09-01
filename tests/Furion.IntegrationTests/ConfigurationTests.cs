using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace Furion.IntegrationTests;

/// <summary>
/// 配置模块集成测试
/// </summary>
public class ConfigurationTests : IClassFixture<WebApplicationFactory<TestProject.FakeStartup>>
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
    public ConfigurationTests(ITestOutputHelper output,
        WebApplicationFactory<TestProject.FakeStartup> factory)
    {
        _output = output;
        _factory = factory;
    }

    /// <summary>
    /// 测试乱码
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestGarbledCode")]
    public async Task TestGarbledCode(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        Assert.Equal("一个应用程序框架，您可以将它集成到任何 .NET/C# 应用程序中。", content);
    }

    /// <summary>
    /// 测试节点是否存在
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestExists", "ConfigurationTest:Enum")]
    public async Task TestExists(string url, string path)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(path), Encoding.UTF8);
        httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/json");
        var content = await _factory.PostAsStringAsync(url, httpContent);
        _output.WriteLine($"{content}");

        Assert.True(bool.Parse(content));
    }

    /// <summary>
    /// 测试读取字符串
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestString")]
    public async Task TestString(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        Assert.Equal("string", content);
    }

    /// <summary>
    /// 测试Int
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestInt")]
    public async Task TestInt(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        Assert.Equal(1, int.Parse(content));
    }

    /// <summary>
    /// 测试Boolean
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestBoolean")]
    public async Task TestBoolean(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        Assert.True(bool.Parse(content));
    }

    /// <summary>
    /// 测试Decimal
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestDecimal")]
    public async Task TestDecimal(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        Assert.Equal(28.1M, decimal.Parse(content));
    }

    /// <summary>
    /// 测试数组
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestArray")]
    public async Task TestArray(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        var result = JsonSerializer.Deserialize<int[]>(content);

        Assert.Equal(4, result!.Last());
    }

    /// <summary>
    /// 测试数组
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestArray2")]
    public async Task TestArray2(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        var result = JsonSerializer.Deserialize<string[]>(content);

        Assert.Equal("value3", result!.Last());
    }

    /// <summary>
    /// 测试对象
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestObject")]
    public async Task TestObject(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        var result = JsonSerializer.Deserialize<Furion.TestProject.ObjectModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.Equal("Furion", result!.Name);
    }

    /// <summary>
    /// 测试枚举
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestEnum")]
    public async Task TestEnum(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        var result = JsonSerializer.Deserialize<Furion.TestProject.EnumModel>(content);

        Assert.Equal(Furion.TestProject.EnumModel.Male, result);
    }

    /// <summary>
    /// 测试枚举
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestEnum2")]
    public async Task TestEnum2(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        var result = JsonSerializer.Deserialize<Furion.TestProject.EnumModel>(content);

        Assert.Equal(Furion.TestProject.EnumModel.Female, result);
    }

    /// <summary>
    /// 测试字典
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestDictionary")]
    public async Task TestDictionary(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        var result = JsonSerializer.Deserialize<Dictionary<string, string>>(content);

        Assert.Equal("value3", result!.Last().Value);
    }

    /// <summary>
    /// 测试内存配置
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestMemory")]
    public async Task TestMemory(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        Assert.Equal("MemoryValue", content);
    }

    /// <summary>
    /// 测试key-per-file 配置
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ConfigurationTests/TestKeyPerFile")]
    public async Task TestKeyPerFile(string url)
    {
        var content = await _factory.PostAsStringAsync(url);
        _output.WriteLine($"{content}");

        var result = JsonSerializer.Deserialize<string[]>(content);

        Assert.Equal("key-per-file3", result!.Last());
    }
}