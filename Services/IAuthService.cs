namespace BackendAPI.Services
{
    public interface IAuthService
    {
        Task<string> GetAccessTokenAsync();
        Task<string> ExchangeAuthCodeWithTokenAsync(string authCode);
    }
}
