// Create a builder by specifying the application and main window.
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCVDemo;
using OpenCVDemo.Models;
using System.Reflection;

var builder = WpfApplication<App, MainWindow>.CreateBuilder(args);

Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => typeof(ObservableObject).IsAssignableFrom(t) ||
                    (typeof(BaseStep).IsAssignableFrom(t) && !t.IsAbstract))
        .ToList()
        .ForEach(t => builder.Services.AddTransient(t));

// Build and run the application.
var app = builder.Build();
await app.RunAsync();
