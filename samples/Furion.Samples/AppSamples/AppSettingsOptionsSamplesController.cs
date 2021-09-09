using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Furion.Samples.AppSamples;

/// <summary>
/// App 模块 AppSettingsOptions 使用示例
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class AppSettingsOptionsSamplesController : ControllerBase
{
    private readonly IOptions<AppSettingsOptions> _options;
    private readonly IOptionsSnapshot<AppSettingsOptions> _optionsSnapshot;
    private readonly IOptionsMonitor<AppSettingsOptions> _optionsMonitor;
    public AppSettingsOptionsSamplesController(IOptions<AppSettingsOptions> options
        , IOptionsSnapshot<AppSettingsOptions> optionsSnapshot
        , IOptionsMonitor<AppSettingsOptions> optionsMonitor)
    {
        _options = options;
        _optionsSnapshot = optionsSnapshot;
        _optionsMonitor = optionsMonitor;
    }

    [HttpPost]
    public void Tests()
    {
        // 配置更改不会刷新
        var appSettings1 = _options.Value;
        Console.WriteLine(appSettings1.EnvironmentVariablesPrefix);

        // 配置更改后下次请求应用
        var appSettings2 = _optionsSnapshot.Value;
        Console.WriteLine(appSettings2.EnvironmentVariablesPrefix);

        // 配置更改后，每次调用都能获取最新配置
        var appSettings3 = _optionsMonitor.CurrentValue;
        Console.WriteLine(appSettings3.EnvironmentVariablesPrefix);
    }
}