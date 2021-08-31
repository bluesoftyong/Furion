using Furion.Options;

namespace Furion;

/// <summary>
/// App 全局应用对象配置
/// </summary>
[AppOptions(_sectionKey)]
public sealed class AppSettingsOptions : IAppOptions<AppSettingsOptions>
{
    /// <summary>
    /// 节点 Key
    /// </summary>
    internal const string _sectionKey = "AppSettings";

    /// <summary>
    /// 环境变量配置 Key 前缀
    /// </summary>
    internal const string _environmentVariablesPrefix = "FURION_";

    /// <summary>
    /// 环境变量配置 Key 前缀
    /// </summary>
    public string EnvironmentVariablesPrefix { get; set; } = _environmentVariablesPrefix;

    /// <summary>
    /// 后期配置
    /// </summary>
    /// <param name="options"></param>
    void IAppOptions<AppSettingsOptions>.PostConfigure(AppSettingsOptions options)
    {
    }
}