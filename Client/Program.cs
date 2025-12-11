using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WalOMat.Client;
using WalOMat.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Quiz services
builder.Services.AddScoped<QuizDataService>();
builder.Services.AddScoped<QuizStateService>();
builder.Services.AddScoped<MatchingService>();

// Localization (singleton so language persists across navigation)
builder.Services.AddSingleton<LocalizationService>();

// Share service (for API calls)
builder.Services.AddScoped<ShareService>();

await builder.Build().RunAsync();
