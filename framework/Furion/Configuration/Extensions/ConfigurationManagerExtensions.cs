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
    /// <remarks>包含自动扫描、环境配置</remarks>
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

        var supportExts = new[] { ".json", ".xml", ".ini" };
        var parameterRegex = new Regex(@"\s+(?<parameter>\boptional\b|\breloadOnChange\b|\bincludeEnvironment\b)\s*=\s*(?<value>\btrue\b|\bfalse\b)");
        Array.ForEach(userConfigurationFiles, item => AddConfigurationFiles(configurationBuilder, environment, item, parameterRegex, supportExts));

        // 添加配置
        static void AddConfigurationFiles(IConfigurationBuilder configurationBuilder, IHostEnvironment environment, string item, Regex parameterRegex, string[] supportExts)
        {
            // 校验配置选项格式是否正确
            var isConfigureParameters = parameterRegex.IsMatch(item);
            var itemSplits = item.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!isConfigureParameters && itemSplits.Length > 1)
                throw new InvalidCastException($"The `{item}` is not a valid configuration format.");

            // 判断是否有效拓展名
            var firstSplit = itemSplits[0];
            var ext = Path.GetExtension(firstSplit);
            if (string.IsNullOrWhiteSpace(ext) || !supportExts.Contains(ext, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only extension named `.json; .xml; .ini;` path.");

            string fileName;
            // 解析绝对路径
            var fileFullPath = firstSplit[0] switch
            {
                '&' => Path.Combine(AppContext.BaseDirectory, fileName = firstSplit[1..]),
                '/' => fileName = firstSplit[1..],
                '@' => Path.Combine(environment.ContentRootPath, fileName = firstSplit[1..]),
                _ => fileName = Path.Combine(AppContext.BaseDirectory, firstSplit)
            };

            // 判断文件格式是否正确 xxx[.{environment}].(json|xml|.ini)
            var fileNameSplits = fileName.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (!(fileNameSplits.Length == 2 || fileNameSplits.Length == 3))
                throw new InvalidOperationException($"The `{fileName}` is not in a valid format of `xxx[.{{environment}}].(json|xml|.ini)`.");

            // 填充配置参数
            bool optional = true, reloadOnChange = true, includeEnvironment = true;
            if (isConfigureParameters)
            {
                foreach (Match match in parameterRegex.Matches(item))
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
        }

        return configurationBuilder;
    }
}