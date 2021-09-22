using System.ComponentModel.DataAnnotations;

namespace Furion.UnitTests;

public class TestValidationOptions
{
    public string? Name { get; set; }
    [Range(2, 100)]
    public int Age { get; set; }
    public int Stars { get; set; }
}