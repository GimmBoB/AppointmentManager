using AppointmentManager.API.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentController: ControllerBase
{
    private static readonly IDictionary<string, AppointmentDto> Cache = new Dictionary<string, AppointmentDto>();

    public AppointmentController()
    { }
    
    [HttpPost]
    public async Task<ActionResult> AddAsync(AppointmentDto appointment)
    {
        Cache.TryAdd(appointment.Name, appointment);

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<AppointmentDto>>> GetAsync()
    {
        var result = Cache.Values;

        return Ok(result);
    }
}