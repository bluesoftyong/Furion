using Furion.ConfigurationSamples;

namespace Microsoft.Extensions.Configuration;

/// <summary>
/// txt 配置拓展
/// </summary>
public static class TxtConfigurationExtensions
{
    public static IConfigurationBuilder AddTxtConfiguration(this IConfigurationBuilder builder, Action<TxtOptions> optionsAction)
    {
        return builder.Add(new TxtConfigurationSource(optionsAction));
    }
}