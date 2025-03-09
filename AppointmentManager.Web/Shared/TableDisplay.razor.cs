using Microsoft.AspNetCore.Components;

namespace AppointmentManager.Web.Shared;

public partial class TableDisplay
{
    [Parameter] public RenderFragment ChildContent { get; set; }
}