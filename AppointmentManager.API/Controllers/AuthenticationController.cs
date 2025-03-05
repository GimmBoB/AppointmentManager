using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Models;
using AppointmentManager.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

public class AuthenticationController : ApplicationControllerBase
{
    private readonly AuthenticationService _service;

    public AuthenticationController(AuthenticationService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public Task<ActionResult> LoginAsync(LoginDto dto, CancellationToken ct) =>
        GetResultAsync<TokenDto>(() => _service.LoginAsync(dto, ct));
    
    [HttpPost("refresh")]
    public Task<ActionResult> RefreshAsync(RefreshDto dto, CancellationToken ct) =>
        GetResultAsync<TokenDto>(() => _service.RefreshAsync(dto, ct));
}