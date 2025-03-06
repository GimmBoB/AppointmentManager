using AppointmentManager.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace AppointmentManager.Web.Shared;

public partial class CustomMainContent
{
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private CustomStateProvider StateProvider { get; set; }
    [Parameter] public RenderFragment? Body { get; set; }

    private ErrorBoundary? _errorBoundary;
    
    
    protected override void OnParametersSet()
    {
        _errorBoundary?.Recover();
    }

    private void AddSnackBar(string errorMessage, Severity severity = Severity.Error)
    {
        Snackbar.Add(errorMessage, severity);
    }
}