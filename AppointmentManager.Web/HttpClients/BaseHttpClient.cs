using AppointmentManager.Web.Extensions;

namespace AppointmentManager.Web.HttpClients;

public abstract class BaseHttpClient
{
    private readonly HttpClient _httpClient;
    // todo retry policy mit refresh

    protected BaseHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<TResult?> PostAsJsonAsync<TResult>(string uri, object input)
        where TResult : class
    {
        var result =await _httpClient.PostAsJsonAsync(uri, input);

        result.EnsureSuccessStatusCode();
        
        return await result.DeserializeContent<TResult>() ?? default;
    }
}