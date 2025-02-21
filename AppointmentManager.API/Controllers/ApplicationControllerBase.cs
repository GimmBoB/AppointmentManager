using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class ApplicationControllerBase : ControllerBase
{
    // TODO Fotos hochladen; Authentication; Emails verschicken
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
                return itemResult.Added
                    ? Created(BuildCreateUri(itemResult.Id.Value), itemResult.Item)
                    : Ok(itemResult.Item);
            }

            return Ok();
        }

        if (apiResult.Errors.Count > 0)
            return BadRequest(apiResult.Errors);
        
        return NotFound();
    }
    
    private string BuildCreateUri(Guid id)
    {
        var uriBuilder = new UriBuilder(Request.GetDisplayUrl());
        uriBuilder.Path += $"/{id}";
        return uriBuilder.Uri.ToString();
    }
}