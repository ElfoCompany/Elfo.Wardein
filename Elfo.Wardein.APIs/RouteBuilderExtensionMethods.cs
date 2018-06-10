using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using Elfo.Wardein.Core;

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

                        //TODO: what if service not found?
                        new WardeinInstance().RestartService(serviceName);
                        return context.Response.WriteAsync($"Service {serviceName} restarted");
                    });
        }
    }
}
