namespace Furion.Samples.AppSamples;

/// <summary>
/// 已注册的接口
/// </summary>
public interface IRegisterService
{
}

/// <summary>
/// 已注册的实例
/// </summary>
public class RegisterService : IRegisterService, ITransientService
{
}