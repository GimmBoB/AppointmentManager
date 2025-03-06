using System.Net;
using AppointmentManager.Shared;
using AppointmentManager.Web.Extensions;

namespace AppointmentManager.Web.HttpClients;

public abstract class BaseHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly Version _defaultRequestVersion = HttpVersion.Version11;
    private readonly HttpVersionPolicy _defaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;    // todo retry policy mit refresh

    protected BaseHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

        var responseMessage = await _httpClient.SendAsync(message, ct);
        
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

        var responseMessage = await _httpClient.SendAsync(message, ct);
        
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

        var responseMessage = await _httpClient.SendAsync(message, ct);
        
        responseMessage.EnsureSuccessStatusCode();
    }
}