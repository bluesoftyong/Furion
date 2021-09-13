using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Furion.UnitTests;

/// <summary>
/// Configuration 模块配置源测试
/// </summary>
public class ConfigurationSourceTests
{
    /// <summary>
    /// 测试 JSON 配置文件
    /// </summary>
    [Fact]
    public void TestJsonConfigurationSource()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();

        builder.Configuration.AddFile("&ConfigurationTests/Files/json-file.json");

        using var app = builder.Build();
        var services = app.Services;

        var configuration = services.GetRequiredService<IConfiguration>();
        configuration["JSON"].Should().BeEquivalentTo("Value");
    }

    /// <summary>
    /// 测试 Xml 配置文件
    /// </summary>
    [Fact]
    public void TestXmlConfigurationSource()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();

        builder.Configuration.AddFile("&ConfigurationTests/Files/xml-file.xml");

        using var app = builder.Build();
        var services = app.Services;

        var configuration = services.GetRequiredService<IConfiguration>();
        configuration["XML"].Should().BeEquivalentTo("XML_VALUE");
    }

    /// <summary>
    /// 测试 Ini 配置文件
    /// </summary>
    [Fact]
    public void TestIniConfigurationSource()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();

        builder.Configuration.AddFile("&ConfigurationTests/Files/ini-file.ini");

        using var app = builder.Build();
        var services = app.Services;

        var configuration = services.GetRequiredService<IConfiguration>();
        configuration["INI"].Should().BeEquivalentTo("VALUE");
        configuration["Test:Title"].Should().BeEquivalentTo("Furion");
        configuration["Log:Default:Level"].Should().BeEquivalentTo("Information");
    }

    /// <summary>
    /// 测试 内存 配置
    /// </summary>
    [Fact]
    public void TestMemoryConfigurationSource()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
        {
            {"MEMORY", "VALUE"},
            {"Position:Title", "FURION"},
        });

        using var app = builder.Build();
        var services = app.Services;

        var configuration = services.GetRequiredService<IConfiguration>();
        configuration["MEMORY"].Should().BeEquivalentTo("VALUE");
        configuration["Position:Title"].Should().BeEquivalentTo("Furion");
    }

    /// <summary>
    /// 测试 Key-per 配置
    /// </summary>
    [Fact]
    public void TestKey_Per_FileConfigurationSource()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();

        builder.Configuration.AddKeyPerFile(Path.Combine(AppContext.BaseDirectory, "ConfigurationTests/Files/key-per-file"));

        using var app = builder.Build();
        var services = app.Services;

        var configuration = services.GetRequiredService<IConfiguration>();
        configuration["key"].Should().BeEquivalentTo("value");
        configuration["name"].Should().BeEquivalentTo("Furion");
        configuration["layer:title"].Should().BeEquivalentTo("Layer");
    }

    /// <summary>
    /// 测试不支持的文件配置
    /// </summary>
    [Fact]
    public void TestUnsupportConfigurationSource()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();
        builder.Configuration.Invoking<IConfigurationBuilder>(builder =>
        {
            builder.AddFile("&ConfigurationTests/Files/file.txt");
        }).Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// 测试自定义配置提供程序
    /// </summary>
    [Fact]
    public void TestCustomizeConfigurationSource()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();
        builder.Configuration.AddTXTConfiguration(options =>
        {
            options.Path = Path.Combine(AppContext.BaseDirectory, "ConfigurationTests/Files/file.txt");
        });

        using var app = builder.Build();
        var services = app.Services;

        var configuration = services.GetRequiredService<IConfiguration>();
        configuration["TXT"].Should().BeEquivalentTo("VALUE");
        configuration["Other:Title"].Should().BeEquivalentTo("FURION");
    }
}