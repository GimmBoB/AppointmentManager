using AppointmentManager.Web.Models;
using AppointmentManager.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace AppointmentManager.Web.Pages;

public partial class Login
{
    [Inject] private CustomStateProvider StateProvider {get; set; }
    [Inject] private NavigationManager Navigation { get; set; }

    private readonly LoginDto _loginDto = new();
    private bool _showOverlay;
    
    private async Task LoginAsync(EditContext arg)
    {
        try
        {
            _showOverlay = true;
            StateHasChanged();
            
            await StateProvider.LoginAsync(_loginDto, CancellationToken.None);
            
            Navigation.TryNavigateToReturnUrl();
        }
        finally
        {
            _showOverlay = true;
        }
    }
}