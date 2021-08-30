namespace Furion.Options;

/// <summary>
/// 选项依赖接口
/// </summary>
/// <typeparam name="TOptions"></typeparam>
public interface IAppOptions<TOptions>
    where TOptions : class
{
    /// <summary>
    /// 后置配置
    /// </summary>
    /// <param name="options"></param>
    void PostConfigure(TOptions options);
}