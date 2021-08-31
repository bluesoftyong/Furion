using Furion;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Microsoft.Extensions.Configuration;

/// <summary>
/// 配置主机构建器拓展类
/// </summary>
public static class ConfigurationManagerExtensions
{
    /// <summary>
    /// 添加应用配置
    /// </summary>
    /// <param name="configurationManager">配置管理对象</param>
    /// <param name="environment">环境对象</param>
    /// <returns></returns>
    public static ConfigurationManager AddAppConfiguration(this ConfigurationManager configurationManager, IHostEnvironment environment)
    {
        var configuration = configurationManager as IConfiguration;
        var configurationBuilder = configurationManager as IConfigurationBuilder;

        configurationBuilder.Configure(configuration, environment);

        return configurationManager;
    }

    /// <summary>
    /// 添加配置文件
    /// </summary>
    /// <param name="configurationManager">配置管理对象</param>
    /// <param name="environment">环境对象</param>
    /// <param name="filePath">文件路径或配置语法路径</param>
    /// <param name="optional">是否可选配置文件</param>
    /// <param name="reloadOnChange">变更刷新</param>
    /// <param name="includeEnvironment">包含环境</param>
    /// <returns></returns>
    public static ConfigurationManager AddConfigurationFile(this ConfigurationManager configurationManager, IHostEnvironment environment, string filePath, bool optional = true, bool reloadOnChange = true, bool includeEnvironment = true)
    {
        var configurationBuilder = configurationManager as IConfigurationBuilder;

        configurationBuilder.AddConfigurationFile(environment, filePath, optional, reloadOnChange, includeEnvironment);

        return configurationManager;
    }

    /// <summary>
    /// 添加配置文件
    /// </summary>
    /// <param name="configurationBuilder">配置构建器</param>
    /// <param name="environment">环境对象</param>
    /// <param name="filePath">文件路径或配置语法路径</param>
    /// <param name="optional">是否可选配置文件</param>
    /// <param name="reloadOnChange">变更刷新</param>
    /// <param name="includeEnvironment">包含环境</param>
    /// <returns></returns>
    public static IConfigurationBuilder AddConfigurationFile(this IConfigurationBuilder configurationBuilder, IHostEnvironment environment, string filePath, bool optional = true, bool reloadOnChange = true, bool includeEnvironment = true)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));

        var supportExts = new[] { ".json", ".xml", ".ini" };
        var parameterRegex = new Regex(@"\s+(?<parameter>\boptional\b|\breloadOnChange\b|\bincludeEnvironment\b)\s*=\s*(?<value>\btrue\b|\bfalse\b)");

        // 校验配置选项格式是否正确
        var isConfigureParameters = parameterRegex.IsMatch(filePath);
        var itemSplits = filePath.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (!isConfigureParameters && itemSplits.Length > 1)
            throw new InvalidCastException($"The `{filePath}` is not a valid configuration format.");

        // 判断是否有效拓展名
        var firstSplit = itemSplits[0];
        var ext = Path.GetExtension(firstSplit);
        if (string.IsNullOrWhiteSpace(ext) || !supportExts.Contains(ext, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only extension named `.json; .xml; .ini;` path.");

        string fileName;
        // 解析绝对路径
        var fileFullPath = firstSplit[0] switch
        {
            '&' or '.' => Path.Combine(AppContext.BaseDirectory, fileName = firstSplit[1..]),
            '/' or '!' => fileName = firstSplit[1..],
            '@' or '~' => Path.Combine(environment.ContentRootPath, fileName = firstSplit[1..]),
            _ => fileName = Path.Combine(AppContext.BaseDirectory, firstSplit)
        };

        Trace.WriteLine(fileFullPath);

        // 判断文件格式是否正确 xxx[.{environment}].(json|xml|.ini)
        var fileNameSplits = fileName.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (!(fileNameSplits.Length == 2 || fileNameSplits.Length == 3))
            throw new InvalidOperationException($"The `{fileName}` is not in a valid format of `xxx[.{{environment}}].(json|xml|.ini)`.");

        // 填充配置参数
        if (isConfigureParameters)
        {
            foreach (Match match in parameterRegex.Matches(filePath))
            {
                var value = bool.Parse(match.Groups["value"].Value);
                switch (match.Groups["parameter"].Value)
                {
                    case "optional":
                        optional = value;
                        break;
                    case "reloadOnChange":
                        reloadOnChange = value;
                        break;
                    case "includeEnvironment":
                        includeEnvironment = value;
                        break;
                    default:
                        break;
                }
            }
        }

        var withEnvironmentFileName = $"{fileNameSplits[0]}.{environment.EnvironmentName}{ext}";
        var isEnvironmentFileItem = fileNameSplits.Length == 3 && fileNameSplits[1].Equals(environment.EnvironmentName, StringComparison.OrdinalIgnoreCase);
        var environmentFileFullPath = includeEnvironment || isEnvironmentFileItem ? Path.Combine(Path.GetDirectoryName(fileFullPath)!, withEnvironmentFileName) : default;

        // 添加配置
        switch (ext.ToLower())
        {
            case ".json":
                if (fileNameSplits.Length == 2) configurationBuilder.AddJsonFile(fileFullPath, optional, reloadOnChange);
                if (includeEnvironment || isEnvironmentFileItem)
                {
                    configurationBuilder.AddJsonFile(environmentFileFullPath, optional, reloadOnChange);
                }
                break;
            case ".xml":
                if (fileNameSplits.Length == 2) configurationBuilder.AddXmlFile(fileFullPath, optional, reloadOnChange);
                if (includeEnvironment || isEnvironmentFileItem)
                {
                    configurationBuilder.AddXmlFile(environmentFileFullPath, optional, reloadOnChange);
                }
                break;
            case ".ini":
                if (fileNameSplits.Length == 2) configurationBuilder.AddIniFile(fileFullPath, optional, reloadOnChange);
                if (includeEnvironment || isEnvironmentFileItem)
                {
                    configurationBuilder.AddIniFile(environmentFileFullPath, optional, reloadOnChange);
                }
                break;
            default:
                break;
        }

        return configurationBuilder;
    }

    /// <summary>
    /// 配置 配置对象构建器
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="configuration"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    internal static IConfigurationBuilder Configure(this IConfigurationBuilder configurationBuilder, IConfiguration configuration, IHostEnvironment environment)
    {
        // 添加 Furion 框架环境变量配置支持
        configurationBuilder.AddEnvironmentVariables(prefix: configuration.GetValue($"{AppSettingsOptions._sectionKey}:{nameof(AppSettingsOptions.EnvironmentVariablesPrefix)}", AppSettingsOptions._environmentVariablesPrefix))
                            .AddCustomizeConfigurationFiles(configuration, environment);

        Trace.WriteLine(string.Join(";\n", configuration.AsEnumerable().Select(c => $"{c.Key}={c.Value}")));

        return configurationBuilder;
    }

    /// <summary>
    /// 添加自定义配置
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    internal static IConfigurationBuilder AddCustomizeConfigurationFiles(this IConfigurationBuilder configurationBuilder, IConfiguration configuration, IHostEnvironment environment)
    {
        var userConfigurationFiles = configuration.GetSection($"{AppSettingsOptions._sectionKey}:{nameof(AppSettingsOptions.CustomizeConfigurationFiles)}").Get<string[]>() ?? Array.Empty<string>();
        if (userConfigurationFiles.Length == 0) return configurationBuilder;

        Array.ForEach(userConfigurationFiles, filePath => configurationBuilder.AddConfigurationFile(environment, filePath));

        return configurationBuilder;
    }
}