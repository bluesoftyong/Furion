using Microsoft.AspNetCore.Mvc;

namespace Furion.TestProject;

/// <summary>
/// 配置模块集成测试 RESTful Api
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class ConfigurationTests : ControllerBase
{
    /// <summary>
    /// 构造函数
    /// </summary>
    private readonly IConfiguration _configuration;
    public ConfigurationTests(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 测试乱码
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public string TestGarbledCode()
    {
        return _configuration["TestSettings:Description"];
    }

    /// <summary>
    /// 测试节点是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [HttpPost]
    public bool TestExists([FromBody] string key)
    {
        return _configuration.Exists(key);
    }

    /// <summary>
    /// 测试读取字符串
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public string TestString()
    {
        return _configuration.Get<string>("ConfigurationTest:String");
    }

    /// <summary>
    /// 测试Int
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public int TestInt()
    {
        return _configuration.Get<int>("ConfigurationTest:Int");
    }

    /// <summary>
    /// 测试Boolean
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public bool TestBoolean()
    {
        return _configuration.Get<bool>("ConfigurationTest:Boolean");
    }

    /// <summary>
    /// 测试Decimal
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public decimal TestDecimal()
    {
        return _configuration.Get<decimal>("ConfigurationTest:Number");
    }

    /// <summary>
    /// 测试数组
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public int[] TestArray()
    {
        return _configuration.Get<int[]>("ConfigurationTest:Array");
    }

    /// <summary>
    /// 测试数组
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public string[] TestArray2()
    {
        return _configuration.Get<string[]>("ConfigurationTest:Array2");
    }

    /// <summary>
    /// 测试对象
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ObjectModel TestObject()
    {
        return _configuration.Get<ObjectModel>("ConfigurationTest:Object");
    }

    /// <summary>
    /// 测试枚举
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public EnumModel TestEnum()
    {
        return _configuration.Get<EnumModel>("ConfigurationTest:Enum");
    }

    /// <summary>
    /// 测试枚举
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public EnumModel TestEnum2()
    {
        return _configuration.Get<EnumModel>("ConfigurationTest:Enum2");
    }

    /// <summary>
    /// 测试字典
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public Dictionary<string, string> TestDictionary()
    {
        return _configuration.Get<Dictionary<string, string>>("ConfigurationTest:Dictionary");
    }

    /// <summary>
    /// 测试内存配置
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public string TestMemory()
    {
        return _configuration.Get<string>("Memory:Item");
    }

    /// <summary>
    /// 测试key-per-file 配置
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public string[] TestKeyPerFile()
    {
        return new[] { _configuration["key"], _configuration["key2"], _configuration["key:name"] };
    }
}

public class ObjectModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public enum EnumModel
{
    Male,
    Female
}