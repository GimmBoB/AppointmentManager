using AppointmentManager.Shared;
using AppointmentManager.Web.config;
using AppointmentManager.Web.HttpClients;
using AppointmentManager.Web.Models;
using AppointmentManager.Web.Services;
using AppointmentManager.Web.Validation;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using MudExtensions.Services;

var builder = WebApplication.CreateBuilder(args);

var apiConfig = new ApiConfig();
builder.Configuration.GetSection(nameof(ApiConfig)).Bind(apiConfig);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ThemeStateProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddScoped<IBaseValidator<Appointment>, AppointmentValidator>();
builder.Services.AddScoped<IBaseValidator<AppointmentCategory>, AppointmentCategoryValidator>();
builder.Services.AddScoped<IBaseValidator<Timeslot>, TimeslotValidator>();
builder.Services.AddHttpClient<ApplicationManagerApiClient>(client =>
{
    client.BaseAddress = new Uri(apiConfig.BaseAddress);
});
builder.Services.AddHttpClient<AuthenticationClient>(client =>
{
    client.BaseAddress = new Uri(apiConfig.BaseAddress);
});

builder.Services.AddScoped<CustomStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomStateProvider>());
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
    config.SnackbarConfiguration.MaxDisplayedSnackbars = 4;
});

builder.Services.AddMudExtensions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();