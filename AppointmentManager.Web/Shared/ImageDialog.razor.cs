using Microsoft.AspNetCore.Components;

namespace AppointmentManager.Web.Shared;

public partial class ImageDialog
{
    [Parameter] public IEnumerable<string> Images { get; set; } = Array.Empty<string>();
}