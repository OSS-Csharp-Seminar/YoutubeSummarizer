using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using YoutubeSummarizerWeb.Client.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddClient(new Uri("http://localhost:5017"));

await builder.Build().RunAsync();
