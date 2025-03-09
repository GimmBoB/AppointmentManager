using System.Security.Claims;
using AppointmentManager.Shared;
using AppointmentManager.Web.HttpClients;
using AppointmentManager.Web.Models;
using Blazored.LocalStorage;
using Core.Extensions.CollectionRelated;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace AppointmentManager.Web.Services;

public class CustomStateProvider : AuthenticationStateProvider
{
    private readonly ISnackbar _snackbar;
    private readonly ILocalStorageService _localStorageService;
    private readonly ApplicationManagerApiClient _apiClient;
    private readonly NavigationManager _navigation;

    private bool _isAuthenticated;
    private string _accessToken = string.Empty;
    private string _refreshToken = string.Empty;
    private const string RefreshStorageKey = "refreshToken";

    public string AccessToken => _accessToken;
    
    public CustomStateProvider(ApplicationManagerApiClient apiClient, NavigationManager navigation, ISnackbar snackbar, ILocalStorageService localStorageService)
    {
        _apiClient = apiClient;
        _navigation = navigation;
        _snackbar = snackbar;
        _localStorageService = localStorageService;
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
    
    public async Task LoginAsync(LoginDto loginParameters, CancellationToken ct)
    {
        try
        {
            var result = await _apiClient.LoginAsync(loginParameters, ct);
            await result.ContinueWithAsync(
                async token =>
                {
                    _isAuthenticated = true;
                    _accessToken = token.AccessToken;
                    _refreshToken = token.RefreshToken;
                    await _localStorageService.SetItemAsync(RefreshStorageKey, _refreshToken, ct);
                    return token;
                },
                async errors =>
                {
                    _isAuthenticated = false;
                    _snackbar.Add($"An error occured: {errors.ToSeparatedString("; ")}", Severity.Warning);
                    await _localStorageService.RemoveItemAsync(RefreshStorageKey, ct);
                    return TokenDto.Empty;
                },
                async () =>
                {
                    _isAuthenticated = false;
                    await _localStorageService.RemoveItemAsync(RefreshStorageKey, ct);
                    return TokenDto.Empty;
                });
            
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        catch(Exception e)
        {
            _isAuthenticated = false;
            await _localStorageService.RemoveItemAsync(RefreshStorageKey, ct);
            _snackbar.Add(e.Message, Severity.Error);
        }
    }
    
    public async Task RefreshAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_refreshToken) && await _localStorageService.ContainKeyAsync(RefreshStorageKey))
                _refreshToken = await _localStorageService.GetItemAsync<string>(RefreshStorageKey) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(_refreshToken))
                return;

            var result = await _apiClient.RefreshAsync(_refreshToken);

            await result.ContinueWithAsync(
                async token =>
                {
                    _isAuthenticated = true;
                    _accessToken = token.AccessToken;
                    _refreshToken = token.RefreshToken;
                    await _localStorageService.SetItemAsync(RefreshStorageKey, _refreshToken);
                    return token;
                },
                async errors =>
                {
                    _isAuthenticated = false;
                    await _localStorageService.RemoveItemAsync(RefreshStorageKey);
                    throw new HttpRequestException(errors.ToSeparatedString("; "));
                },
                async () =>
                {
                    _isAuthenticated = false;
                    await _localStorageService.RemoveItemAsync(RefreshStorageKey);
                    return TokenDto.Empty;
                });
            
        }
        catch
        {
            _isAuthenticated = false;
            await _localStorageService.RemoveItemAsync(RefreshStorageKey);
            throw;
        }
        
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync(CancellationToken ct)
    {
        _isAuthenticated = false;
        _accessToken = string.Empty;
        _refreshToken = string.Empty;
        
        await _localStorageService.RemoveItemAsync(RefreshStorageKey, ct);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}