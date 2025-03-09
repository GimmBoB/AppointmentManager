using System.Net;
using System.Net.Http.Headers;
using AppointmentManager.Shared;
using AppointmentManager.Web.Extensions;
using AppointmentManager.Web.Services;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Wrap;

namespace AppointmentManager.Web.HttpClients;

public abstract class BaseHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly Version _defaultRequestVersion = HttpVersion.Version11;
    private readonly HttpVersionPolicy _defaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
    private readonly CustomStateProvider _stateProvider;
    private readonly AsyncPolicyWrap<HttpResponseMessage> _policyWrap;

    protected BaseHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // _stateProvider = stateProvider;
        
        // var refreshPolicy = Policy<HttpResponseMessage>
        //     .HandleResult(resp => resp.StatusCode == HttpStatusCode.Unauthorized)
        //     .RetryAsync(2, async (_, _) =>
        //     {
        //         await _stateProvider.RefreshAsync();
        //         SetTokenToHeader();
        //     });
        // var serverOfflinePolicy = Policy<HttpResponseMessage>
        //     .Handle<HttpRequestException>()
        //     .OrResult(x => x.StatusCode is >= HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
        //     .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3));

        // _policyWrap = refreshPolicy.WrapAsync(serverOfflinePolicy);

    }

    private void SetTokenToHeader()
    {
        // clear header value
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer");

        // set token to request header
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _stateProvider.AccessToken);
    }

    protected async Task<Option<TResult>> PostAsJsonAsync<TInput, TResult>(string uri, TInput input, CancellationToken ct)
        where TResult : class
        where TInput : class
    {
        return await SendAsync<TInput, TResult>(HttpMethod.Post, uri, input, ct);
    }
    
    protected async Task<Option<TResult>> GetAsJsonAsync<TResult>(string uri, CancellationToken ct)
        where TResult : class
    {
        return await SendAsync<TResult>(HttpMethod.Get, uri, ct);
    }

    protected async Task PostAsync(string uri, HttpContent content, CancellationToken ct)
    {
        await SendAsync(HttpMethod.Post, uri, content, ct);
    }

    private async Task<Option<TResult>> SendAsync<TInput, TResult>(HttpMethod method, string uri, TInput value, CancellationToken ct)
        where TResult : class
        where TInput : class
    {
        var message = new HttpRequestMessage(method, uri)
        {
            Content = JsonContent.Create(value, mediaType: null),
            Version = _defaultRequestVersion,
            VersionPolicy = _defaultVersionPolicy
        };

        var responseMessage = await _policyWrap.ExecuteAsync(() => _httpClient.SendAsync(message, ct));
        
        if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
        {
            var errors = await responseMessage.DeserializeContent<string[]>() ?? Array.Empty<string>();
            return Option<TResult>.Error(errors);
        }

        responseMessage.EnsureSuccessStatusCode();
        
        var result = await responseMessage.DeserializeContent<TResult>() ?? default;

        return result is not null ? Option<TResult>.Some(result) : Option<TResult>.None;
    }
    
    private async Task<Option<TResult>> SendAsync<TResult>(HttpMethod method, string uri, CancellationToken ct)
        where TResult : class
    {
        var message = new HttpRequestMessage(method, uri)
        {
            Version = _defaultRequestVersion,
            VersionPolicy = _defaultVersionPolicy
        };

        var responseMessage = await _policyWrap.ExecuteAsync(() => _httpClient.SendAsync(message, ct));
        
        if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
        {
            var errors = await responseMessage.DeserializeContent<string[]>() ?? Array.Empty<string>();
            return Option<TResult>.Error(errors);
        }

        responseMessage.EnsureSuccessStatusCode();
        
        var result = await responseMessage.DeserializeContent<TResult>() ?? default;

        return result is not null ? Option<TResult>.Some(result) : Option<TResult>.None;
    }
    
    private async Task SendAsync(HttpMethod method, string uri, HttpContent content, CancellationToken ct)
    {
        var message = new HttpRequestMessage(method, uri)
        {
            Version = _defaultRequestVersion,
            VersionPolicy = _defaultVersionPolicy,
            Content = content
        };

        var responseMessage = await _policyWrap.ExecuteAsync(() => _httpClient.SendAsync(message, ct));
        
        responseMessage.EnsureSuccessStatusCode();
    }
}