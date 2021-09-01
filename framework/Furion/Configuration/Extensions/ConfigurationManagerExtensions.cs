// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion;
using Furion.ObjectExtensions;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.Extensions.Configuration;

/// <summary>
/// 配置主机构建器拓展类
/// </summary>
public static class ConfigurationManagerExtensions
{
    /// <summary>
    /// 添加文件配置
    /// </summary>
    /// <param name="configurationManager">配置管理对象</param>
    /// <param name="filePath">文件路径或配置语法路径</param>
    /// <param name="environment">环境对象</param>
    /// <param name="optional">是否可选配置文件</param>
    /// <param name="reloadOnChange">变更刷新</param>
    /// <param name="includeEnvironment">包含环境</param>
    /// <returns></returns>
    public static ConfigurationManager AddFile(this ConfigurationManager configurationManager, string filePath, IHostEnvironment? environment = default, bool optional = true, bool reloadOnChange = true, bool includeEnvironment = true)
    {
        var configurationBuilder = configurationManager as IConfigurationBuilder;

        configurationBuilder.AddFile(filePath, environment, optional, reloadOnChange, includeEnvironment);

        return configurationManager;
    }

    /// <summary>
    /// 添加文件配置
    /// </summary>
    /// <param name="configurationBuilder">配置构建器</param>
    /// <param name="filePath">文件路径或配置语法路径</param>
    /// <param name="environment">环境对象</param>
    /// <param name="optional">是否可选配置文件</param>
    /// <param name="reloadOnChange">变更刷新</param>
    /// <param name="includeEnvironment">包含环境</param>
    /// <returns></returns>
    public static IConfigurationBuilder AddFile(this IConfigurationBuilder configurationBuilder, string filePath, IHostEnvironment? environment = default, bool optional = true, bool reloadOnChange = true, bool includeEnvironment = true)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));

        var parameterRegex = new Regex(@"\s+(?<parameter>\boptional\b|\breloadOnChange\b|\bincludeEnvironment\b)\s*=\s*(?<value>\btrue\b|\bfalse\b)");

        // 校验配置选项格式是否正确
        var defineParameters = parameterRegex.IsMatch(filePath);
        var itemSplits = filePath.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (!defineParameters && itemSplits.Length > 1)
            throw new InvalidCastException($"The `{filePath}` is not a valid configuration format.");

        var firstSplit = itemSplits[0];
        string fileName;
        // 解析绝对路径
        var fileFullPath = firstSplit[0] switch
        {
            '&' or '.' => Path.Combine(AppContext.BaseDirectory, fileName = firstSplit[1..]),
            '/' or '!' => fileName = firstSplit[1..],
            '@' or '~' => Path.Combine(environment is null ? Directory.GetCurrentDirectory() : environment.ContentRootPath, fileName = firstSplit[1..]),
            _ => fileName = Path.Combine(AppContext.BaseDirectory, firstSplit)
        };

        Trace.WriteLine(fileFullPath);

        // 判断文件格式是否正确 xxx[.{environment}].(json|xml|.ini)
        var fileNameSplits = fileName.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (!(fileNameSplits.Length == 2 || fileNameSplits.Length == 3))
            throw new InvalidOperationException($"The `{fileName}` is not in a valid format of `xxx[.{{environment}}].(json|xml|.ini)`.");

        // 填充配置参数
        if (defineParameters)
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

        var fileExtension = Path.GetExtension(fileName);

        // 创建添加配置文件委托
        var addFile = CreateAddProviderFileDelegate(configurationBuilder, fileExtension);
        if (fileNameSplits.Length == 2 || environment is null) addFile(fileFullPath, optional, reloadOnChange);

        // 带环境标识的文件
        if (environment is not null)
        {
            // 是否带环境标识的文件名
            var isWithEnvironmentFile = fileNameSplits.Length == 3 && fileNameSplits[1].Equals(environment.EnvironmentName, StringComparison.OrdinalIgnoreCase);
            // 拼接带环境名的完整路径
            var environmentFileFullPath = includeEnvironment || isWithEnvironmentFile
                                                    ? Path.Combine(Path.GetDirectoryName(fileFullPath)!, $"{fileNameSplits[0]}.{environment.EnvironmentName}{fileExtension}")
                                                    : default;
            if (includeEnvironment || isWithEnvironmentFile) addFile(environmentFileFullPath!, optional, reloadOnChange);
        }

        return configurationBuilder;
    }

    /// <summary>
    /// 添加内存集合配置
    /// </summary>
    /// <param name="configurationManager">配置管理对象</param>
    /// <param name="initialData">集合</param>
    /// <returns></returns>
    public static ConfigurationManager AddInMemoryCollection(this ConfigurationManager configurationManager, IEnumerable<KeyValuePair<string, string>> initialData)
    {
        var configurationBuilder = configurationManager as IConfigurationBuilder;

        configurationBuilder.AddInMemoryCollection(initialData);

        return configurationManager;
    }

    /// <summary>
    /// 添加目录文件 Key-per-file 配置
    /// </summary>
    /// <param name="configurationManager">配置管理对象</param>
    /// <param name="initialData">集合</param>
    /// <returns></returns>
    public static ConfigurationManager AddKeyPerFile(this ConfigurationManager configurationManager, string directoryPath, bool optional = true, bool reloadOnChange = true)
    {
        var configurationBuilder = configurationManager as IConfigurationBuilder;

        configurationBuilder.AddKeyPerFile(directoryPath, optional, reloadOnChange);

        return configurationManager;
    }

    /// <summary>
    /// 配置 配置对象构建器
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="configuration"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    internal static IConfigurationBuilder Configure(this IConfigurationBuilder configurationBuilder, IConfiguration configuration, IHostEnvironment? environment = default)
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
    internal static IConfigurationBuilder AddCustomizeConfigurationFiles(this IConfigurationBuilder configurationBuilder, IConfiguration configuration, IHostEnvironment? environment = default)
    {
        var userConfigurationFiles = configuration.GetSection($"{AppSettingsOptions._sectionKey}:{nameof(AppSettingsOptions.CustomizeConfigurationFiles)}").Get<string[]>() ?? Array.Empty<string>();
        if (userConfigurationFiles.Length == 0) return configurationBuilder;

        Array.ForEach(userConfigurationFiles, filePath => configurationBuilder.AddFile(filePath, environment));

        return configurationBuilder;
    }

    /// <summary>
    /// 创建添加配置文件委托
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="fileExtension"></param>
    /// <returns></returns>
    private static Func<string, bool, bool, IConfigurationBuilder> CreateAddProviderFileDelegate(IConfigurationBuilder configurationBuilder, string fileExtension)
    {
        var supportExts = new[] { ".json", ".xml", ".ini" };
        if (string.IsNullOrWhiteSpace(fileExtension) || !supportExts.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("The file type configuration provider handler was not found.");

        var flag = fileExtension.ToLower()[1..].ToTitleCase();

        // 加载特定程序集中类型对应方法
        var providerAssembly = Assembly.Load($"Microsoft.Extensions.Configuration.{flag}");
        var providerClassType = providerAssembly.GetType($"Microsoft.Extensions.Configuration.{flag}ConfigurationExtensions");
        var providerMethodInfo = providerClassType!.GetTypeInfo().DeclaredMethods
                                                        .First(u => u.IsPublic && u.IsStatic
                                                            && u.Name == $"Add{flag}File" && u.GetParameters().Length == 4);

        // 创建方法委托
        IConfigurationBuilder func(string f, bool o, bool r)
        {
            return providerMethodInfo.CreateDelegate<Func<IConfigurationBuilder, string, bool, bool, IConfigurationBuilder>>()
                    (configurationBuilder, f, o, r);
        }

        return func;
    }
}