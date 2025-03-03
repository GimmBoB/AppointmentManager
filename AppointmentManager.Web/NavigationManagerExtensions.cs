using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace AppointmentManager.Web;

public static class NavigationManagerExtensions
{
    public static void TryNavigateToReturnUrl(this NavigationManager navigation, string fallBackUri = KnownDirections.Home)
    {
        var uri = navigation.ToAbsoluteUri(navigation.Uri);
        navigation.NavigateTo(uri: QueryHelpers.ParseQuery(uri.Query).TryGetValue("returnUrl", out var returnUrl)
            ? returnUrl.ToString()
            : fallBackUri);
    }
    
    public static void NavigateToWithReturnUri(this NavigationManager navigation, string direction)
    {
        navigation.NavigateTo($"{direction}?returnUrl={Uri.EscapeDataString(navigation.Uri)}");
    }
}