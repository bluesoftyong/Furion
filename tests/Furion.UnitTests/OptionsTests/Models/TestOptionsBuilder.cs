using Furion.Options;

namespace Furion.UnitTests;

public class TestOptionsBuilder : IOptionsBuilder<TestOptions>
{
    public void Configure(TestOptions options)
    {
    }

    public void PostConfigure(TestOptions options)
    {
    }

    public bool Validate(TestOptions options)
    {
        return true;
    }
}