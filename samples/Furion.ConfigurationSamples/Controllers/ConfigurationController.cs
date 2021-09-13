using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Furion.ConfigurationSamples.Controllers;

/// <summary>
/// Configuration 模块使用示例
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class ConfigurationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public ConfigurationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 读取 string 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetString()
    {
        return _configuration["String"] + _configuration.Get<string>("String");
    }

    /// <summary>
    /// 读取 bool 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public bool GetBoolean()
    {
        return _configuration.Get<bool>("Boolean");
    }

    /// <summary>
    /// 读取 bool 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public bool GetBoolean2()
    {
        return _configuration.Get<bool>("Boolean2");
    }

    /// <summary>
    /// 读取 int 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public int GetInt()
    {
        return _configuration.Get<int>("Int");
    }

    /// <summary>
    /// 读取 long 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public long GetLong()
    {
        return _configuration.Get<long>("Long");
    }

    /// <summary>
    /// 读取 float 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public float GetFloat()
    {
        return _configuration.Get<float>("Float");
    }

    /// <summary>
    /// 读取 decimal 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public decimal GetDecimal()
    {
        return _configuration.Get<decimal>("Decimal");
    }

    /// <summary>
    /// 读取 enum 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Gender GetEnum()
    {
        return _configuration.Get<Gender>("Enum");
    }

    /// <summary>
    /// 读取 enum 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Gender GetEnum2()
    {
        return _configuration.Get<Gender>("Enum2");
    }

    /// <summary>
    /// 读取 int[] 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public int[] GetArray()
    {
        return _configuration.Get<int[]>("Array");
    }

    /// <summary>
    /// 读取 string[] 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string[] GetArray2()
    {
        return _configuration.Get<string[]>("Array2");
    }

    /// <summary>
    /// 读取 dictionary 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Dictionary<string, string> GetDictionary()
    {
        return _configuration.Get<Dictionary<string, string>>("Dictionary");
    }

    /// <summary>
    /// 读取 object 配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ObjectModel GetObject()
    {
        return _configuration.Get<ObjectModel>("Object");
    }

    /// <summary>
    /// 检查配置是否从存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [HttpPost]
    public bool CheckExists([Required, FromBody] string key)
    {
        return _configuration.Exists(key);
    }
}