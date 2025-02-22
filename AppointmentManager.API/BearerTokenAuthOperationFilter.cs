using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppointmentManager.API;

public class BearerAuthOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attribute = context.MethodInfo.GetCustomAttributes<AuthorizeAttribute>().FirstOrDefault();

        if (attribute != null)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer"
                        },
                        new string[]{}
                    }
                }
            };
        }
    }
}