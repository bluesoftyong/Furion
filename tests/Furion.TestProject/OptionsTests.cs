using Furion.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Furion.TestProject.Controllers;

/// <summary>
/// 选项模块集成测试 RESTful Api
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class OptionsTests : ControllerBase
{
    private readonly IOptions<TestSettingsOptions> _testOptions;
    private readonly IOptionsSnapshot<TestSettingsOptions> _testOptionsSnapshot;
    private readonly IOptionsMonitor<TestSettingsOptions> _testOptionsMonitor;

    private readonly IOptions<Test2SettingsOptions> _test2Options;
    private readonly IOptionsSnapshot<Test2SettingsOptions> _test2OptionsSnapshot;
    private readonly IOptionsMonitor<Test2SettingsOptions> _test2OptionsMonitor;
    public OptionsTests(IOptions<TestSettingsOptions> testOptions
        , IOptionsSnapshot<TestSettingsOptions> testOptionsSnapshot
        , IOptionsMonitor<TestSettingsOptions> testOptionsMonitor

        , IOptions<Test2SettingsOptions> test2Options
        , IOptionsSnapshot<Test2SettingsOptions> test2OptionsSnapshot
        , IOptionsMonitor<Test2SettingsOptions> test2OptionsMonitor)
    {
        _testOptions = testOptions;
        _testOptionsSnapshot = testOptionsSnapshot;
        _testOptionsMonitor = testOptionsMonitor;

        _test2Options = test2Options;
        _test2OptionsSnapshot = test2OptionsSnapshot;
        _test2OptionsMonitor = test2OptionsMonitor;
    }

    /// <summary>
    /// 测试未实现 IAppOptions 选项
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public string[] TestNotImplementOptions()
    {
        return new string[] { _testOptions.Value.Name!, _testOptionsSnapshot.Value.Name!, _testOptionsMonitor.CurrentValue.Name! };
    }

    /// <summary>
    /// 测试实现 IAppOptions 选项
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public string[] TestImplementOptions()
    {
        return new string[] { _test2Options.Value.Name!, _test2OptionsSnapshot.Value.Name!, _test2OptionsMonitor.CurrentValue.Name! };
    }
}

/// <summary>
/// 未实现 IAppOptions 接口测试
/// </summary>
public sealed class TestSettingsOptions
{
    public string? Name { get; set; }
}

/// <summary>
/// 实现 IAppOptions 接口测试
/// </summary>
[AppOptions("TestSettings")]
public sealed class Test2SettingsOptions : IAppOptions<Test2SettingsOptions>
{
    public string? Name { get; set; }

    /// <summary>
    /// 显示实现
    /// </summary>
    /// <param name="options"></param>
    void IAppOptions<Test2SettingsOptions>.PostConfigure(Test2SettingsOptions options)
    {
        options.Name ??= "Furion";
    }
}