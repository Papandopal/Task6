using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Task6Itransition.Services;
using Task6Itransition.Services.Drawers;

namespace Task6Itransition
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<CanvasService>();
            builder.Services.AddScoped<SignalRSettings>();
            builder.Services.AddScoped<SaveSchemeService>();

            await builder.Build().RunAsync();
        }
    }
}
