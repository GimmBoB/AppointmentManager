using System.Net;
using AppointmentManager.Shared;
using AppointmentManager.Web.Models;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using AppointmentManager.Web.Extensions;


namespace AppointmentManager.Web.HttpClients;

public class AuthenticationClient
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _serverNotAvailablePolicy;


    public AuthenticationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _serverNotAvailablePolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(x => x.StatusCode is >= HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 2));

    }
    
    public async Task<Option<TokenDto>> LoginAsync(LoginDto login, CancellationToken ct = default)
    {
        var response =
            await _serverNotAvailablePolicy.ExecuteAsync(() =>
                _httpClient.PostAsJsonAsync("Authentication/login", login, ct));
        
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errors = await response.DeserializeContent<string[]>() ?? Array.Empty<string>();
            return Option<TokenDto>.Error(errors);
        }
        
        response.EnsureSuccessStatusCode();

        var result = await response.DeserializeContent<TokenDto>() ?? default;

        return result is not null ? Option<TokenDto>.Some(result) : Option<TokenDto>.None;
    }

    public async Task<Option<TokenDto>> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        var refresh = new RefreshDto(refreshToken);

        var response = await _serverNotAvailablePolicy.ExecuteAsync(() =>
            _httpClient.PostAsJsonAsync("Authentication/refresh", refresh, ct));
        
        response.EnsureSuccessStatusCode();
        
        var result = await response.DeserializeContent<TokenDto>() ?? default;

        return result is not null ? Option<TokenDto>.Some(result) : Option<TokenDto>.None;
    }
}