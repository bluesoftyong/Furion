namespace Microsoft.Extensions.Configuration;

/// <summary>
/// IConfiguration 对象拓展
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// 判断配置是否存在
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool Exists(this IConfiguration configuration, string key)
    {
        return configuration.GetSection(key).Exists();
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="configuration"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T Get<T>(this IConfiguration configuration, string key)
    {
        return configuration.GetSection(key).Get<T>();
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="configuration"></param>
    /// <param name="key"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static T Get<T>(this IConfiguration configuration, string key, Action<BinderOptions> configureOptions)
    {
        return configuration.GetSection(key).Get<T>(configureOptions);
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="key"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object Get(this IConfiguration configuration, string key, Type type)
    {
        return configuration.GetSection(key).Get(type);
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="key"></param>
    /// <param name="type"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static object Get(this IConfiguration configuration, string key, Type type, Action<BinderOptions> configureOptions)
    {
        return configuration.GetSection(key).Get(type, configureOptions);
    }
}