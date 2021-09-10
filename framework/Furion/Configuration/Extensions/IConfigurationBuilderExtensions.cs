// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.Xml;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Extensions.Configuration;

/// <summary>
/// IConfigurationBuilder 接口拓展
/// </summary>
public static class IConfigurationBuilderExtensions
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
    public static ConfigurationManager AddFile(this ConfigurationManager configurationManager, string filePath, IHostEnvironment? environment = default, bool optional = true, bool reloadOnChange = false, bool includeEnvironment = false)
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
    public static IConfigurationBuilder AddFile(this IConfigurationBuilder configurationBuilder, string filePath, IHostEnvironment? environment = default, bool optional = true, bool reloadOnChange = false, bool includeEnvironment = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        var parameterRegex = new Regex(@"\s+(?<parameter>\b\w+\b)\s*=\s*(?<value>\btrue\b|\bfalse\b)");

        // 校验配置选项格式是否正确
        var defineParameters = parameterRegex.IsMatch(filePath);
        var itemSplits = filePath.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (!defineParameters && itemSplits.Length > 1)
        {
            throw new InvalidCastException($"The `{filePath}` is not a valid configuration format.");
        }

        var firstSplit = itemSplits[0];
        string fileName;
        // 解析绝对路径
        var fileFullPath = firstSplit[0] switch
        {
            '&' or '.' => Path.Combine(AppContext.BaseDirectory, fileName = firstSplit[1..]),
            '/' or '!' => fileName = firstSplit[1..],
            '@' or '~' => Path.Combine(environment is null ? Directory.GetCurrentDirectory() : environment.ContentRootPath, fileName = firstSplit[1..]),
            _ => Path.Combine(environment is null ? Directory.GetCurrentDirectory() : environment.ContentRootPath, fileName = firstSplit)
        };

        Trace.WriteLine(fileFullPath);

        // 判断文件格式是否正确 xxx[.{environment}].(json|xml|.ini)
        var fileNameSplits = fileName.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (!(fileNameSplits.Length == 2 || fileNameSplits.Length == 3))
        {
            throw new InvalidOperationException($"The `{fileName}` is not in a valid format of `xxx[.{{environment}}].(json|xml|.ini)`.");
        }

        // 填充配置参数
        if (defineParameters)
        {
            var parameters = parameterRegex.Matches(filePath)
                                                               .ToDictionary(u => u.Groups["parameter"].Value, u => bool.Parse(u.Groups["value"].Value));

            parameters.TryGetValue(nameof(optional), out optional);
            parameters.TryGetValue(nameof(reloadOnChange), out reloadOnChange);
            parameters.TryGetValue(nameof(includeEnvironment), out includeEnvironment);
        }

        // 添加配置文件
        configurationBuilder.Add(CreateFileConfigurationSource(fileFullPath, optional, reloadOnChange));

        // 环境对象不为空且文件不带环境配置
        if (fileNameSplits.Length == 2 && environment is not null)
        {
            // 是否带环境标识的文件名
            var isWithEnvironmentFile = fileNameSplits.Length == 3 && fileNameSplits[1].Equals(environment.EnvironmentName, StringComparison.OrdinalIgnoreCase);

            // 添加带环境配置文件
            if (includeEnvironment || isWithEnvironmentFile)
            {
                var environmentFileFullPath = Path.Combine(Path.GetDirectoryName(fileFullPath)!, $"{fileNameSplits[0]}.{environment.EnvironmentName}{Path.GetExtension(fileName)}");
                configurationBuilder.Add(CreateFileConfigurationSource(environmentFileFullPath, optional, reloadOnChange));
            }
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
    public static ConfigurationManager AddKeyPerFile(this ConfigurationManager configurationManager, string directoryPath, bool optional = true, bool reloadOnChange = false)
    {
        var configurationBuilder = configurationManager as IConfigurationBuilder;

        configurationBuilder.AddKeyPerFile(directoryPath, optional, reloadOnChange);

        return configurationManager;
    }

    /// <summary>
    /// 分析配置文件名并返回真实绝对路径
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>返回文件绝对路径</returns>
    private static string ResolveRealAbsolutePath(string fileName)
    {
        // 如果文件名包含 : 符号，则认为是一个绝对路径，windows 系统路径
        if (fileName.IndexOf(':') > -1)
        {
            return fileName;
        }

        // 获取文件名首个字符
        var firstChar = fileName[0];

        // 拼接绝对路径
        return firstChar switch
        {
            '&' or '.' => Path.Combine(AppContext.BaseDirectory, fileName[1..]),
            '/' or '!' => fileName[1..],
            '@' or '~' or _ => Path.Combine(Directory.GetCurrentDirectory(), fileName[1..])
        };
    }

    /// <summary>
    /// 根据文件路径创建文件配置源
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="optional">可选文件，设置 true 跳过文件存在检查</param>
    /// <param name="reloadOnChange">是否监听文件更改</param>
    /// <returns>FileConfigurationSource实例</returns>
    private static FileConfigurationSource CreateFileConfigurationSource(string filePath, bool optional = true, bool reloadOnChange = false)
    {
        // 获取文件拓展名
        var fileExtension = Path.GetExtension(filePath).ToLower();

        // 创建受支持的文件配置源实例，仅支持 .json/.xml/.ini 拓展名
        FileConfigurationSource? fileConfigurationSource = fileExtension switch
        {
            ".json" => new JsonConfigurationSource { Path = filePath, Optional = optional, ReloadOnChange = reloadOnChange },
            ".xml" => new XmlConfigurationSource { Path = filePath, Optional = optional, ReloadOnChange = reloadOnChange },
            ".ini" => new IniConfigurationSource { Path = filePath, Optional = optional, ReloadOnChange = reloadOnChange },
            _ => throw new InvalidOperationException($"Cannot create a file `{fileExtension}` configuration source for this file type.")
        };

        // 根据文件配置源解析对应文件配置提供程序
        fileConfigurationSource.ResolveFileProvider();

        return fileConfigurationSource;
    }
}