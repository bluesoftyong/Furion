using Microsoft.AspNetCore.Mvc;

namespace Furion.TestProject.Controllers;

/// <summary>
/// App 全局应用对象集成测试 RESTful Api
/// </summary>
[ApiController]
[Route("[controller]")]
public class AppTests : ControllerBase
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public AppTests()
    {
    }
}