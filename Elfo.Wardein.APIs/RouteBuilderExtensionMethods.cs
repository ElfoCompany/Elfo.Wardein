using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using Elfo.Wardein.Core;
using Microsoft.Extensions.DependencyInjection;
using Elfo.Wardein.Core.Helpers;

namespace Elfo.Wardein.APIs
{
    public static class RouteBuilderExtensionMethods
    {
        public static void ConfigureWardeinRouting(this RouteBuilder thisRouteBuilder)
        {
            //TODO: how to manage logs?
            thisRouteBuilder.MapGet("restartservice/{name}",
                context =>
                    {
                        //TODO: how to manage exceptions?
                        string serviceName = context.GetRouteValue("name").ToString();

                        //TODO: what if service not found? We need to manage exceptions: Service not found, exceptions while stopping/starting service
                        new WindowsServiceHelper(serviceName).Restart();
                        return context.Response.WriteAsync($"Service {serviceName} restarted");
                    });
        }
    }
}
