using Furion.Options;

namespace Furion;

/// <summary>
/// App 全局应用对象配置
/// </summary>
[AppOptions("AppSettings")]
public sealed class AppSettingsOptions : IAppOptions<AppSettingsOptions>
{
    /// <summary>
    /// 后置配置
    /// </summary>
    /// <param name="options"></param>
    public void PostConfigure(AppSettingsOptions options)
    {
    }
}