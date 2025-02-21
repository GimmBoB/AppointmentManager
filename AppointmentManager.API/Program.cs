using AppointmentManager.API.Database;
using AppointmentManager.API.Extensions;
using AppointmentManager.API.QuartzJobs;
using AppointmentManager.API.Security;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAppointmentManagerServices(builder.Configuration);

builder.Services.AddQuartz(options =>
{
    // options.UseMicrosoftDependencyInjectionJobFactory();

    // Register the job, loading the schedule from configuration
    options.AddJobAndTrigger<EnsureValidCertificateJob>(builder.Configuration);
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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