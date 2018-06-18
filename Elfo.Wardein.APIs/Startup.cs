using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Elfo.Wardein.APIs
{
    public class Startup
    {
        public static ServiceProvider ServiceProvider { get; private set; }
        public void Configure(IApplicationBuilder app)
        {
            var routeBuilder = new RouteBuilder(app);
            routeBuilder.ConfigureWardeinRouting();
            app.UseRouter(routeBuilder.Build());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
