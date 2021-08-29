using Microsoft.AspNetCore.Mvc.Testing;

namespace Furion.IntegrationTests;

public static class WebApplicationFactoryExtensions
{
    /// <summary>
    /// WebApplicationFactory PostAsStringAsync 拓展
    /// </summary>
    /// <typeparam name="TEntryPoint"></typeparam>
    /// <param name="factory"></param>
    /// <param name="url"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static async Task<string> PostAsStringAsync<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory, string url, HttpContent? content = default)
        where TEntryPoint : class
    {
        using var httpClient = factory.CreateClient();
        using var response = await httpClient.PostAsync($"{url}", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}

