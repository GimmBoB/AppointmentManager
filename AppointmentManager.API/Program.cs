using System.Security.Claims;
using AppointmentManager.API;
using AppointmentManager.API.Database;
using AppointmentManager.API.Extensions;
using AppointmentManager.API.QuartzJobs;
using AppointmentManager.API.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<BearerAuthOperationFilter>();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme.",
        In = ParameterLocation.Header,
        Name = HeaderNames.Authorization,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
});

builder.Services.AddAppointmentManagerServices(builder.Configuration);

builder.Services.AddQuartz(options =>
{
    // options.UseMicrosoftDependencyInjectionJobFactory();

    // Register the job, loading the schedule from configuration
    options.AddJobAndTrigger<EnsureValidCertificateJob>(builder.Configuration);
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddAuthentication(options =>
{
    options.AddScheme(JwtBearerDefaults.AuthenticationScheme, x => x.HandlerType = typeof(BearerTokenHandler));
});
        
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireClaim(ClaimTypes.NameIdentifier)
        .RequireClaim(ClaimTypes.Email)
        .RequireClaim(ClaimTypes.Name)
        .Build();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MigrateDatabase();

var serviceProvider =
    app.Services
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope().ServiceProvider;

SeedCertificateStore.Initialize(serviceProvider);
await SeedDatabase.SeedAsync(serviceProvider);

app.Run();