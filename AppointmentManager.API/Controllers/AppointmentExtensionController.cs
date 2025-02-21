using AppointmentManager.API.ControllerServices;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

public class AppointmentExtensionController : ApplicationControllerBase
{
    private readonly AppointmentExtensionService _service;

    public AppointmentExtensionController(AppointmentExtensionService service)
    {
        _service = service;
    }

    [HttpPost("{appointId:guid}")]
    public Task AddFileAsync(Guid appointId, [FromForm] IFormFile file, CancellationToken ct) =>
        GetResultAsync(() => _service.AddFileAsync(appointId, file, ct));

    [HttpGet("{id:guid}/image")]
    public async Task<ActionResult> GetFileAsync(Guid id, CancellationToken ct)
    {
        var imageInfo = await _service.GetImageInfoAsync(id, ct);

        if (imageInfo is null)
            return NotFound();
        
        return new FileStreamResult(new FileStream(imageInfo.FilePath, FileMode.Open, FileAccess.Read),
            imageInfo.ContentType);
    }
}