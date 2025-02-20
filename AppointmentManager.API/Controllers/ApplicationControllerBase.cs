using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class ApplicationControllerBase : ControllerBase
{
    protected async Task<ActionResult> GetResultAsync<T>(Func<Task<ApiResult>> func)
        where T : class
    {
        ApiResult apiResult;

        try
        {
            apiResult = await func();
        }
        catch (Exception e)
        {
            return Problem(statusCode: (int)HttpStatusCode.InternalServerError, detail: e.Message,
                title: "Unhandled Error");
        }
        
        if (apiResult.Success)
        {
            if (apiResult is ItemApiResult<T> itemResult)
            {
                return Ok(itemResult.Item);
            }

            return Ok();
        }

        if (apiResult.Errors.Count > 0)
            return BadRequest(apiResult.Errors);
        
        return NotFound();
    }
}