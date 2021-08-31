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
    public string? EnvironmentVariablesPrefix { get; set; }

    /// <summary>
    /// 配置文件列表
    /// </summary>
    /// <remarks>
    /// <para>限定特定配置文件：[...]; 不加载配置：null</para>
    /// <para>@: 程序根目录; /：绝对路径; &：程序执行目录（bin，默认值）</para>
    /// <para>支持 *.json;*.xml;*.ini</para>
    /// <para>支持配置 includeEnvironment、optional 和 reloadOnChange</para>
    /// </remarks>
    /// <example>["furion.json", "@furion.json", "&furion.json", "/D:/furion.json", "furion.json optional=true reloadOnChange=true"]</example>
    public string[]? CustomizeConfigurationFiles { get; set; }

    /// <summary>
    /// 后期配置
    /// </summary>
    /// <param name="options"></param>
    void IAppOptions<AppSettingsOptions>.PostConfigure(AppSettingsOptions options)
    {
        options.EnvironmentVariablesPrefix ??= _environmentVariablesPrefix;
    }
}