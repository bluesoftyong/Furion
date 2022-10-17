﻿using Furion.Application.Persons;
using Furion.RemoteRequest;
using Furion.RemoteRequest.Extensions;
using Furion.UnifyResult;

namespace Furion.Application;

public class TestModuleServices : IDynamicApiController
{
    private readonly IHttp _http;

    public TestModuleServices(IHttp http)
    {
        _http = http;
    }

    [HttpPost]
    public IActionResult UploadFileAsync(IFormFile file)
    {
        return new ContentResult() { Content = file.FileName };
    }

    [HttpPost]
    public IActionResult UploadMulitiFileAsync(List<IFormFile> files)
    {
        return new ContentResult() { Content = string.Join(',', files.Select(u => u.FileName)) };
    }

    /// <summary>
    /// 测试单文件上传
    /// </summary>
    /// <returns></returns>
    public async Task<string> TestSingleFileProxy()
    {
        var bytes = File.ReadAllBytes("image.png");
        var result = await _http.TestSingleFileProxyAsync(HttpFile.Create("file", bytes, "image.png"));
        var fileName = await result.Content.ReadAsStringAsync();

        return fileName;
    }

    /// <summary>
    /// 测试多文件上传
    /// </summary>
    /// <returns></returns>
    public async Task<string> TestMultiFileProxy()
    {
        var bytes = File.ReadAllBytes("image.png");
        var result = await _http.TestMultiFileProxyAsync(HttpFile.CreateMultiple("files", (bytes, "image1.png"), (bytes, "image2.png")));
        var fileName = await result.Content.ReadAsStringAsync();

        return fileName;
    }

    /// <summary>
    /// 测试单文件上传（字符串方式）
    /// </summary>
    /// <returns></returns>
    public async Task<string> TestSingleFileProxyString()
    {
        var bytes = File.ReadAllBytes("image.png");

        var result = await "https://localhost:44316/api/test-module/upload-file".SetContentType("multipart/form-data")
                            .SetFiles(HttpFile.Create("file", bytes, "image.png")).PostAsync();

        var fileName = await result.Content.ReadAsStringAsync();

        return fileName;
    }

    /// <summary>
    /// 测试多文件上传（字符串方式）
    /// </summary>
    /// <returns></returns>
    public async Task<string> TestMultiFileProxyString()
    {
        var bytes = File.ReadAllBytes("image.png");
        var result = await "https://localhost:44316/api/test-module/upload-muliti-file".SetContentType("multipart/form-data")
                            .SetFiles(HttpFile.CreateMultiple("files", (bytes, "image1.png"), (bytes, "image2.png"))).PostAsync();
        var fileName = await result.Content.ReadAsStringAsync();

        return fileName;
    }

    public async Task<string> TestRequestEncode([FromQuery] string ip)
    {
        var url = $"http://whois.pconline.com.cn/ipJson.jsp?ip={ip}";
        var resultStr = await url.GetAsStringAsync();
        return resultStr;
    }

    public async Task<PersonDto> TestSerial()
    {
        var result = await "https://localhost:44316/api/person/1".GetAsAsync<RESTfulResult<PersonDto>>();

        return result.Data;
    }

    public async Task<string> TestBaidu()
    {
        var url = $"https://www.baidu.com";
        var resultStr = await url.GetAsStringAsync();
        return resultStr;
    }


    public void 测试高频远程请求()
    {
        Parallel.For(0, 5000, (i) =>
        {
            "https://www.baidu.com".GetAsStringAsync();
        });
    }

    /// <summary>
    /// 测试文件流上传
    /// </summary>
    /// <returns></returns>
    public async Task<string> TestSingleFileSteamProxyString()
    {
        var fileStream = new FileStream("image.png", FileMode.Open);

        var result = await "https://localhost:44316/api/test-module/upload-file".SetContentType("multipart/form-data")
                            .SetFiles(HttpFile.Create("file", fileStream, "image.png")).PostAsync();

        var fileName = await result.Content.ReadAsStringAsync();

        await fileStream.DisposeAsync();

        return fileName;
    }

    /// <summary>
    /// 测试单文件流上传
    /// </summary>
    /// <returns></returns>
    public async Task<string> TestSingleFileStreamProxy()
    {
        var fileStream = new FileStream("image.png", FileMode.Open);
        var result = await _http.TestSingleFileProxyAsync(HttpFile.Create("file", fileStream, "image.png"));
        var fileName = await result.Content.ReadAsStringAsync();

        await fileStream.DisposeAsync();
        return fileName;
    }
}