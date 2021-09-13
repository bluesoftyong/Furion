using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Furion.UnitTests;

/// <summary>
/// Configuration 模块添加配置文件测试
/// </summary>
public class AddFileTests
{
    [Fact]
    public void TestAddFile()
    {
        var builder = WebApplication.CreateBuilder();

        // 测试添加配置
        builder.Configuration.Invoking(config =>
        {
            config.AddFile("furion.json");
            config.AddFile("furion.xml");
            config.AddFile("furion.ini");

            config.AddFile("&furion.json");
            config.AddFile("@furion.xml");
            config.AddFile(".furion.ini");
            config.AddFile("/D:/furion.ini");
            config.AddFile("&furion.ini");
            config.AddFile("~furion.ini");
            config.AddFile("!furion.ini");

            config.AddFile("furion.a.json");
            config.AddFile("furion.b.c.json");

            config.AddFile("furion.json includeEnvironment=true");
            config.AddFile("furion.json optional=true");
            config.AddFile("furion.json reloadOnChange=true");
            config.AddFile("furion.json includeEnvironment=true optional=true reloadOnChange=false");
            config.AddFile("furion.json includeEnvironment=true optional=true reloadOnChange=false xxx=true");
        }).Should().NotThrow();

        // 测试错误配置
        builder.Configuration.Invoking(config =>
        {
            config.AddFile("furion.txt");
            config.AddFile("furion.cs");

            config.AddFile("furion.json optional=false");
        }).Should().Throw<InvalidOperationException>();
    }
}