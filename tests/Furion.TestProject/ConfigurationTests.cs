using Microsoft.AspNetCore.Mvc;

namespace Furion.TestProject;

/// <summary>
/// 配置模块集成测试 RESTful Api
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class ConfigurationTests : ControllerBase
{
    /// <summary>
    /// 构造函数
    /// </summary>
    private readonly IConfiguration _configuration;
    public ConfigurationTests(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public string TestGarbledCode()
    {
        return _configuration["TestSettings:Description"];
    }
}