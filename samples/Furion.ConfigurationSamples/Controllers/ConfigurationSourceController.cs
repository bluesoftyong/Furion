using Microsoft.AspNetCore.Mvc;

namespace Furion.ConfigurationSamples.Controllers;

/// <summary>
/// Configuration 模块各种提供程序使用示例
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class ConfigurationSourceController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public ConfigurationSourceController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 读取环境配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetEnvironmentValue()
    {
        return _configuration["Env:Name"] + " " + _configuration["Env:Title"];
    }

    /// <summary>
    /// 读取命令行配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetCommandLineValue()
    {
        return _configuration["CMD"] + " " + _configuration["CMD:Title"];
    }

    /// <summary>
    /// 读取内存配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetMemoryValue()
    {
        return _configuration["Memory"] + " " + _configuration["Memory:Title"];
    }

    /// <summary>
    /// 读取Xml配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetXmlValue()
    {
        return _configuration["XML"] + " " + _configuration["Other:Title"];
    }

    /// <summary>
    /// 读取Ini配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetIniValue()
    {
        return _configuration["INI"] + " " + _configuration["INI:Title"] + " " + _configuration["INI:Default:Level"];
    }

    /// <summary>
    /// 读取key-per-file配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetKeyPerFileValue()
    {
        return _configuration["kpf_key"] + " " + _configuration["kpf_name"] + " " + _configuration["kpf_layer:title"];
    }

    /// <summary>
    /// 读取 Txt 自定义配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetTxtValue()
    {
        return _configuration["TXT"] + " " + _configuration["Txt:Title"];
    }
}