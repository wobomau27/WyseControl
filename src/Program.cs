using WyseControl.Services;
using WyseControl.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IVpnService, WireGuardService>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<SystemInfoService>();
builder.Services.AddSingleton<IVnStatService, VnStatService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapStatusEndpoints();
app.MapVpnEndpoints();

//app.Urls.Add("http://0.0.0.0:5000");

app.Run();