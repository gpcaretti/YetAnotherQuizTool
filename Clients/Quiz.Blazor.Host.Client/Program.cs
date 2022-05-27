using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Quiz.Application;

namespace Quiz.Blazor.Host.Client {
    public class Program {

        //private const string StandardClientName = "Quiz.Blazor.Host.ServerAPI";
        //private const string AnonymousClientName = "ServerAPI.NoAuthenticationClient";

        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // This is the standard http client requiring access tokens
            builder.Services.AddHttpClient(QuizConstants.ClientNames.Standard, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // This is a second HttpClient that does not require authentication (allows anonymous requests)
            // See: https://docs.microsoft.com/en-us/aspnet/core/security/blazor/webassembly/additional-scenarios?view=aspnetcore-3.1#unauthenticated-or-unauthorized-web-api-requests-in-an-app-with-a-secure-default-client
            builder.Services.AddHttpClient(QuizConstants.ClientNames.Anonymous, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(QuizConstants.ClientNames.Standard));

            builder.Services.AddApiAuthorization();

            await builder.Build().RunAsync();
        }
    }
}