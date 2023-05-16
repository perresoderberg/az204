using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.FileProviders;
using TestTool204.API.Controllers;
using TestTool204.Application.Interfaces;
using TestTool204.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers().
AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddSingleton<IMainService, MainService>();

//builder.Host.ConfigureAppConfiguration(x => { x.AddJsonFile("appsettings.json"); });

builder.Services.AddSingleton<IIOService, IOService>();
builder.Services.AddSingleton<IIOService, IOService>();
builder.Services.AddSingleton<QandAService>();
builder.Services.AddSingleton<MainController>();


if (builder.Services.All(x => x.ServiceType != typeof(HttpClient)))
{
    builder.Services.AddScoped(
        s =>
        {
            var navigationManager = s.GetRequiredService<NavigationManager>();
            return new HttpClient
            {
                BaseAddress = new Uri(navigationManager.BaseUri)
            };
        });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.UseWebAssemblyDebugging();
}

if (!app.Environment.IsDevelopment())
{

    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.MapDefaultControllerRoute();

app.MapRazorPages();


app.MapBlazorHub();

app.MapFallbackToPage("/_Host");

app.MapControllers();

app.Run();

