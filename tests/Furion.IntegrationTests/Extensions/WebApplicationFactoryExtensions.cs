using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;

namespace Furion.IntegrationTests;

public static class WebApplicationFactoryExtensions
{
    /// <summary>
    /// 发送 POST 请求
    /// </summary>
    /// <typeparam name="TEntryPoint"></typeparam>
    /// <param name="factory"></param>
    /// <param name="url"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static async Task PostAsync<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory, string url, HttpContent? content = default)
        where TEntryPoint : class
    {
        using var httpClient = factory.CreateClient();
        using var response = await httpClient.PostAsync($"{url}", content);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// WebApplicationFactory PostAsStringAsync 拓展
    /// </summary>
    /// <typeparam name="TEntryPoint"></typeparam>
    /// <param name="factory"></param>
    /// <param name="url"></param>
    /// <param name="content"></param>
    /// <param name="ensure"></param>
    /// <returns></returns>
    public static async Task<string> PostAsStringAsync<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory, string url, HttpContent? content = default, bool ensure = true)
        where TEntryPoint : class
    {
        using var httpClient = factory.CreateClient();
        using var response = await httpClient.PostAsync($"{url}", content);
        if (ensure) response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}