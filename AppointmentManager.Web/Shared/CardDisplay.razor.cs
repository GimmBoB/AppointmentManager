using Microsoft.AspNetCore.Components;

namespace AppointmentManager.Web.Shared;

public partial class CardDisplay
{
    [Parameter] public RenderFragment ChildContent { get; set; }
}