using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppointmentManager.Web.Extensions;

public static class HttpResponseExtensions
{
    public static async Task<T?> DeserializeContent<T>(this HttpResponseMessage response)
        where T : class
    {
        var body = await response.Content.ReadAsStringAsync();
            
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        var result = JsonSerializer.Deserialize<T>(body, options);
        return result;
    }
}