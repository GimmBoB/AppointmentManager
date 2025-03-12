using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppointmentManager.Web.Shared;

public partial class CustomDialog
{
    [CascadingParameter] public MudDialogInstance? MudDialog { get; set; }

    [Parameter] public string? ContentText { get; set; }

    [Parameter] public string? SubmitButtonText { get; set; }
    [Parameter] public string? CancelButtonText { get; set; }
    [Parameter] public Color Color { get; set; }

    private void Submit() => MudDialog?.Close(DialogResult.Ok(true));

    private void Cancel() => MudDialog?.Cancel();
}