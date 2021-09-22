using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Furion.UnitTests;

/// <summary>
/// 选项模块测试
/// </summary>
public class OptionsModuleTests
{
    /// <summary>
    /// 测试选项绑定配置
    /// </summary>
    [Fact]
    public void TestBindConfiguration()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddFile("&OptionsTests/Files/options.json");
        using var app = builder.Build();
        var configuration = app.Configuration;

        // 测试绑定
        var testOptions = new TestOptions();
        configuration.GetSection("OptionsTest").Bind(testOptions);
        testOptions.Name.Should().BeEquivalentTo("Furion");
        testOptions.Age.Should().Be(1);
        testOptions.Stars.Should().Be(7100);
    }

    /// <summary>
    /// 测试通过配置获取选项对象
    /// </summary>
    [Fact]
    public void TestGetOptionsByConfiguration()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddFile("&OptionsTests/Files/options.json");
        using var app = builder.Build();
        var configuration = app.Configuration;

        // 测试获取选项值
        var testOptions = configuration.GetSection("OptionsTest").Get<TestOptions>();
        testOptions.Name.Should().BeEquivalentTo("Furion");
        testOptions.Age.Should().Be(1);
        testOptions.Stars.Should().Be(7100);
    }

    /// <summary>
    /// 测试选项通过 Configure 方式配置并读取
    /// </summary>
    [Fact]
    public void TestConfigureMethod()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddFile("&OptionsTests/Files/options.json");

        // 测试配置选项
        builder.Services.Configure<TestOptions>(builder.Configuration.GetSection("OptionsTest"));
        using var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<TestOptions>>();
        // 测试取值
        options.Value.Should().NotBeNull();
        var testOptions = options.Value;
        testOptions.Name.Should().BeEquivalentTo("Furion");
        testOptions.Age.Should().Be(1);
        testOptions.Stars.Should().Be(7100);
    }

    /// <summary>
    /// 测试选项通过 Configure 方式配置并读取（绑定未知属性）
    /// </summary>
    [Fact]
    public void TestConfigureMethod2()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddFile("&OptionsTests/Files/options.json");

        // 测试配置选项
        builder.Services.Configure<TestOptions>(builder.Configuration.GetSection("OptionsTest"), binderOptions =>
        {
            binderOptions.ErrorOnUnknownConfiguration = true;
        });
        using var app = builder.Build();

        // 测试未知属性绑定抛异常
        var options = app.Services.GetRequiredService<IOptions<TestOptions>>();
        options.Invoking(o => o.Value).Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// 测试选项通过 Configure 方式配置并读取（绑定非公开属性）
    /// </summary>
    [Fact]
    public void TestConfigureMethod3()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddFile("&OptionsTests/Files/options.json");

        // 测试配置选项
        builder.Services.Configure<TestOptions>(builder.Configuration.GetSection("OptionsTest"), binderOptions =>
        {
            binderOptions.BindNonPublicProperties = true;
        });
        using var app = builder.Build();

        // 测试未公开属性绑定
        var options = app.Services.GetRequiredService<IOptions<TestOptions>>();
        var testOptions = options.Value;
        testOptions.Private.Should().BeEquivalentTo("Private");
        var privateProperty = typeof(TestOptions).GetProperty("Private2", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        privateProperty.Should().NotBeNull();
        privateProperty?.GetValue(testOptions).Should().NotBeNull().And.Be("Private2");
    }

    /// <summary>
    /// 测试通过绑定方式注册选项
    /// </summary>
    [Fact]
    public void TestBindOptions()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddFile("&OptionsTests/Files/options.json");

        // 测试配置选项
        builder.Services.AddOptions<TestOptions>().Bind(builder.Configuration.GetSection("OptionsTest"));
        using var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<TestOptions>>();
        // 测试取值
        options.Value.Should().NotBeNull();
        var testOptions = options.Value;
        testOptions.Name.Should().BeEquivalentTo("Furion");
        testOptions.Age.Should().Be(1);
        testOptions.Stars.Should().Be(7100);
    }

    /// <summary>
    /// 测试通过 Configure 配置选项
    /// </summary>
    [Fact]
    public void TestConfigureOptions()
    {
        var builder = WebApplication.CreateBuilder();

        // 测试配置选项
        builder.Services.AddOptions<TestOptions>().Configure(options =>
        {
            options.Name = "Furion1";
            options.Age = 2;
            options.Stars = 7111;
        });
        using var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<TestOptions>>();
        // 测试取值
        options.Value.Should().NotBeNull();
        var testOptions = options.Value;
        testOptions.Name.Should().BeEquivalentTo("Furion1");
        testOptions.Age.Should().Be(2);
        testOptions.Stars.Should().Be(7111);
    }

    /// <summary>
    /// 测试通过 Configure 依赖注入配置选项
    /// </summary>
    [Fact]
    public void TestConfigureOptions1()
    {
        var builder = WebApplication.CreateBuilder();

        // 测试配置选项
        builder.Services.AddOptions<TestOptions>().Configure<IWebHostEnvironment>((options, enviroment) =>
        {
            options.Name = "Furion_" + enviroment.EnvironmentName;
            options.Age = 2;
            options.Stars = 7111;
        });
        using var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<TestOptions>>();
        // 测试取值
        options.Value.Should().NotBeNull();
        var testOptions = options.Value;
        testOptions.Name.Should().BeEquivalentTo("Furion_Production");
        testOptions.Age.Should().Be(2);
        testOptions.Stars.Should().Be(7111);
    }

    /// <summary>
    /// 测试特性验证
    /// </summary>
    [Fact]
    public void TestValidateDataAnnotations()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddFile("&OptionsTests/Files/options.json");

        // 测试配置选项
        builder.Services.AddOptions<TestValidationOptions>().Bind(builder.Configuration.GetSection("OptionsTest"))
            .ValidateDataAnnotations();
        using var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<TestValidationOptions>>();
        options.Invoking(o => o.Value).Should().Throw<OptionsValidationException>();
    }

    /// <summary>
    /// 测试复杂验证
    /// </summary>
    [Fact]
    public void TestValidate()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddFile("&OptionsTests/Files/options.json");

        // 测试配置选项
        builder.Services.AddOptions<TestValidationOptions>().Bind(builder.Configuration.GetSection("OptionsTest"))
            .Validate(options =>
            {
                if (options.Age < 2)
                {
                    return false;
                }
                return true;
            }, "validate faild");
        using var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<TestValidationOptions>>();
        options.Invoking(o => o.Value).Should().Throw<OptionsValidationException>();
    }

    /// <summary>
    /// 测试选项后期配置
    /// </summary>
    [Fact]
    public void TestPostConfigure()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddFile("&OptionsTests/Files/options.json");

        // 测试配置选项
        builder.Services.AddOptions<TestOptions>().Bind(builder.Configuration.GetSection("OptionsTest"))
            .PostConfigure<IWebHostEnvironment>((options, enviroment) =>
            {
                options.Name += "_" + enviroment.EnvironmentName;
            });
        using var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<TestOptions>>();
        // 测试取值
        options.Value.Should().NotBeNull();
        var testOptions = options.Value;
        testOptions.Name.Should().BeEquivalentTo("Furion_Production");
        testOptions.Age.Should().Be(1);
        testOptions.Stars.Should().Be(7100);
    }
}