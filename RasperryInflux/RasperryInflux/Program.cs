using InfluxDB3.Client;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet;
using RasperryInflux.Client.Pages;
using RasperryInflux.Components;
using RasperryInflux.Data;
using RasperryInflux.Data.InfluxDB;
using RasperryInflux.Service;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

IConfigurationSection broker = builder.Configuration.GetSection("BrokerHostSettings");


builder.Services.AddSingleton(new MqttFactory().CreateMqttClient());
builder.Services.AddSingleton(new MqttClientOptionsBuilder()
                .WithTcpServer(broker["Host"], Convert.ToInt32(broker["Port"])));

builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddSingleton<IInfluxRepository, InfluxRepository>();
builder.Services.AddHostedService<WorkerRepository>();

builder.Services.AddMudServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(RasperryInflux.Client._Imports).Assembly);

app.Run();
