using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kekiri.Tests.WebApp;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddScoped<IFoo, DefaultFoo>()
            .AddScoped<IBar, DefaultBar>();
    }

    // StartupLoader requires a Configure method on any type passed to UseStartup.
    public void Configure(IApplicationBuilder app)
    {
    }
}
