using AppointmentManager.Shared;
using AppointmentManager.Web.Models;

namespace AppointmentManager.Web.HttpClients;

public class ApplicationManagerApiClient : BaseHttpClient
{
    public ApplicationManagerApiClient(HttpClient httpClient) : base(httpClient)
    {
    }
    
    public async Task<TokenDto?> LoginAsync(LoginDto login)
    {
        return await PostAsJsonAsync<TokenDto>("/login", login);
    }

    public async Task<TokenDto?> RefreshAsync(string? refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        var refresh = new RefreshDto(refreshToken);
        
        return await PostAsJsonAsync<TokenDto>("/refresh", refresh);
    }
}