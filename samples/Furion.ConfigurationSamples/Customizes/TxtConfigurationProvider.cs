using Microsoft.Extensions.FileProviders;

namespace Furion.ConfigurationSamples;

/// <summary>
/// Txt 配置提供器
/// </summary>
public class TxtConfigurationProvider : ConfigurationProvider
{
    public TxtConfigurationProvider(Action<TxtOptions> optionsAction)
    {
        OptionsAction = optionsAction;
    }

    Action<TxtOptions> OptionsAction { get; }

    public override void Load()
    {
        var options = new TxtOptions();
        OptionsAction(options);

        var dic = Path.GetDirectoryName(options.Path);
        var fileinfo = new PhysicalFileProvider(dic).GetFileInfo(Path.GetFileName(options.Path));

        using var stream = fileinfo.CreateReadStream();
        var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        using var reader = new StreamReader(stream);
        while (reader.Peek() > -1)
        {
            var lineText = reader.ReadLine()!;
            if (string.IsNullOrWhiteSpace(lineText.Trim()))
            {
                continue;
            }

            var splits = lineText.Split('=', StringSplitOptions.RemoveEmptyEntries);
            dictionary[splits[0]] = splits[1];
        }

        base.Data = dictionary;
    }
}