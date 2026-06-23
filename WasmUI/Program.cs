using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WasmUI;
using WasmUI.Services;
using WasmUI.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<ITodoClientService, TodoClientService>();

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5044") });

builder.Services.AddHttpClient<ITodoClientService, TodoClientService>(client =>
    client.BaseAddress = new Uri("http://localhost:5044")
);

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

await builder.Build().RunAsync();
