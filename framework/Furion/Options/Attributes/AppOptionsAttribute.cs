namespace Furion.Options;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AppOptionsAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public AppOptionsAttribute()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sectionKey">配置节点 Key</param>
    public AppOptionsAttribute(string sectionKey)
    {
        SectionKey = sectionKey;
    }

    /// <summary>
    /// 配置节点 Key
    /// </summary>
    public string? SectionKey { get; set; }
}