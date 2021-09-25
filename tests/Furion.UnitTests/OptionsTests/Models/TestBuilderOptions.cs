using Furion.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Furion.UnitTests;

[OptionsBuilder("OptionsTest", ValidateOptionsTypes = new[] { typeof(TestBuilderOptionsValidation) })]
public class TestBuilderOptions : IConfigureOptionsBuilder<TestBuilderOptions, IConfiguration>
    , IPostConfigureOptionsBuilder<TestBuilderOptions, IHostEnvironment>
    , IValidateOptionsBuilder<TestBuilderOptions, IConfiguration, IHostEnvironment>
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public int Stars { get; set; }

    void IConfigureOptionsBuilder<TestBuilderOptions, IConfiguration>.Configure(TestBuilderOptions options, IConfiguration dep)
    {
        options.Name = "Furion2";
    }

    void IPostConfigureOptionsBuilder<TestBuilderOptions, IHostEnvironment>.PostConfigure(TestBuilderOptions options, IHostEnvironment dep)
    {
        options.Age = 30;
    }

    bool IValidateOptionsBuilder<TestBuilderOptions, IConfiguration, IHostEnvironment>.Validate(TestBuilderOptions options, IConfiguration dep1, IHostEnvironment dep2)
    {
        return true;
    }
}

public class TestBuilderOptionsValidation : IValidateOptions<TestBuilderOptions>
{
    private readonly IConfiguration _configuration;
    public TestBuilderOptionsValidation(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ValidateOptionsResult Validate(string name, TestBuilderOptions options)
    {
        if (options.Age > 30)
        {
            return ValidateOptionsResult.Fail("Validate Failed");
        }
        return ValidateOptionsResult.Success;
    }
}