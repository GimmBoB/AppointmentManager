using System.Security.Claims;
using AppointmentManager.Web.HttpClients;
using AppointmentManager.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace AppointmentManager.Web.Services;

public class CustomStateProvider : AuthenticationStateProvider
{
    private readonly ApplicationManagerApiClient _apiClient;
    private bool _isAuthenticated; 
    private readonly NavigationManager _navigation;
     // todo beim instanziieren token versuchen aus browser local storage zu holen


    public string? AccessToken { get; private set; } = string.Empty;
    private string? RefreshToken { get; set; } = string.Empty;

    public CustomStateProvider(ApplicationManagerApiClient apiClient, NavigationManager navigation)
    {
        _apiClient = apiClient;
        _navigation = navigation;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        try
        {
            if (_isAuthenticated)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Role, "admin")
                };
                identity = new ClaimsIdentity(claims, "Server authentication");
                
                _navigation.TryNavigateToReturnUrl(fallBackUri: _navigation.Uri);
            }
            else
                await RefreshAsync();
        }
        catch (Exception)
        {
            // ignore
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
    
    public async Task LoginAsync(LoginDto loginParameters)
    {
        try
        {
            var token = await _apiClient.LoginAsync(loginParameters);
            AccessToken = token?.AccessToken;
            RefreshToken = token?.RefreshToken;
            _isAuthenticated = true;
        }
        catch
        {
            _isAuthenticated = false;
            throw;
        }

        
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    
    public async Task RefreshAsync()
    {
        try
        {
            var token = await _apiClient.RefreshAsync(RefreshToken);
            AccessToken = token?.AccessToken;
            RefreshToken = token?.RefreshToken;
            _isAuthenticated = true;
        }
        catch
        {
            _isAuthenticated = false;
            throw;
        }
        
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}