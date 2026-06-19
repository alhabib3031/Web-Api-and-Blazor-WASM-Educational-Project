using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WasmUI;
using WasmUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5044") });

// builder.Services.AddHttpClient<TodoClientService>(client =>
// {
//     client.BaseAddress = new Uri("http://localhost:5044");
// });

// builder.Services.AddScoped(sp => new HttpClient
// {
//     BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
// });

await builder.Build().RunAsync();
