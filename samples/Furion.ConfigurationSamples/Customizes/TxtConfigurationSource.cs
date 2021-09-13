namespace Furion.ConfigurationSamples;

/// <summary>
/// 自定义 Txt 配置源
/// </summary>
public class TxtConfigurationSource : IConfigurationSource
{
    private readonly Action<TxtOptions> _optionsAction;

    public TxtConfigurationSource(Action<TxtOptions> optionsAction)
    {
        _optionsAction = optionsAction;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new TxtConfigurationProvider(_optionsAction);
    }
}