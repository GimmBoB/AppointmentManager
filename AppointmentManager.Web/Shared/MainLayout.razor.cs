using AppointmentManager.Web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppointmentManager.Web.Shared;

public partial class MainLayout
{
    [Inject] private ThemeStateProvider ThemeStateProvider { get; set; }
    [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
    [Inject] private CustomStateProvider StateProvider { get; set; }
    
    private MudThemeProvider? _mudThemeProvider;
    private const string UserPreferenceStorageKey = "userPreference";
    private async Task OnToggledChanged()
    {
        ThemeStateProvider.IsDarkMode = !ThemeStateProvider.IsDarkMode;
        
        await SetUserPreferenceToLocalStorage(ThemeStateProvider.IsDarkMode);
        
        ThemeStateProvider.OnStateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ThemeStateProvider.IsDarkMode = await GetUserPreferenceDarkMode();

            StateHasChanged();
        }
    }
    
    private async Task SetUserPreferenceToLocalStorage(bool isDarkMode)
    {
        if (!await LocalStorage.ContainKeyAsync(UserPreferenceStorageKey))
        {
            await LocalStorage.SetItemAsync(UserPreferenceStorageKey, new UserPreference { IsDarkMode = isDarkMode });
        }
        else
        {
            var userPreference = await LocalStorage.GetItemAsync<UserPreference>(UserPreferenceStorageKey) ?? new UserPreference();
            userPreference.IsDarkMode = isDarkMode;
            await LocalStorage.SetItemAsync(UserPreferenceStorageKey, userPreference);
        }
    }

    private async Task<bool> GetUserPreferenceDarkMode()
    {
        if (await LocalStorage.ContainKeyAsync(UserPreferenceStorageKey))
        {
            var userPreference = await LocalStorage.GetItemAsync<UserPreference>(UserPreferenceStorageKey) ?? new UserPreference();
            return userPreference.IsDarkMode;
        }
        
        var isDarkMode = _mudThemeProvider != null && await _mudThemeProvider.GetSystemPreference();

        await LocalStorage.SetItemAsync(UserPreferenceStorageKey, new UserPreference { IsDarkMode = isDarkMode });
        
        return isDarkMode;
    }

    private Task LogoutAsync() => StateProvider.LogoutAsync(CancellationToken.None);
}