using Microsoft.Extensions.DependencyInjection;
using OpenAIDemo;
using OpenAIDemo.Services;
using System.Net.Http.Headers;

// Create a builder by specifying the application and main window.
var builder = WpfApplication<App, MainWindow>.CreateBuilder(args);
builder.Configuration.AddUserSecrets("OpenAIDemo");
builder.Services.AddHttpClient<OpenAIClient>((services, client) =>
{
    IConfiguration configuration = services.GetRequiredService<IConfiguration>();
    client.BaseAddress = configuration.GetValue<Uri>("OpenAI:BaseUrl");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration.GetValue<string>("OpenAI:ApiKey"));
});

builder.Services.AddTransient<MainWindowViewModel>();

// Build and run the application.
var app = builder.Build();
await app.RunAsync();