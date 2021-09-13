using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Furion.UnitTests;

/// <summary>
/// Configuration 模块读取配置测试
/// </summary>
public class ConfigurationReadTest
{
    /// <summary>
    /// 测试各类型值读取
    /// </summary>
    [Fact]
    public void TestValues()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.AddFile("&ConfigurationTests/Files/values.json");

        using var app = builder.Build();
        var services = app.Services;

        var configuration = services.GetRequiredService<IConfiguration>();

        // 测试各种类型值读取
        configuration.Get<string>("String").Should().Be("String");
        configuration.Get<bool>("Boolean").Should().BeTrue();
        configuration.Get<bool>("Boolean2").Should().BeFalse();
        configuration.Get<int>("Int").Should().Be(2);
        configuration.Get<long>("Long").Should().Be(33333333333333333);
        configuration.Get<float>("Float").Should().Be(-20.2F);
        configuration.Get<decimal>("Decimal").Should().Be(40.32M);
        configuration.Get<Gender>("Enum").Should().Be(Gender.Male);
        configuration.Get<Gender>("Enum2").Should().Be(Gender.Male);
        configuration.Get<int[]>("Array").Should().HaveCount(4);
        configuration.Get<string[]>("Array2").Should().HaveCount(4);
        configuration.Get<Dictionary<string, string>>("Dictionary").Should().HaveCount(3);
        configuration.Get<ObjectModel>("Object").Should().NotBeNull().And.Match<ObjectModel>(o => o.Name == "Furion");
        configuration["Object:Version"].Should().Be("Next");
    }

    /// <summary>
    /// 测试配置是否存在
    /// </summary>
    [Fact]
    public void TestExists()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.AddFile("&ConfigurationTests/Files/values.json");

        using var app = builder.Build();
        var services = app.Services;

        var configuration = services.GetRequiredService<IConfiguration>();

        // 测试键是否存在
        configuration.Exists("Object:Version").Should().BeTrue();
        configuration.Exists("Object:Title").Should().BeFalse();
    }
}