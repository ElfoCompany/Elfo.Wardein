using Microsoft.AspNetCore.Routing;

namespace Elfo.Wardein.APIs
{
    public static class RouteBuilderExtensionMethods
    {
        public static void ConfigureWardeinRouting(this RouteBuilder thisRouteBuilder)
        {
            //TODO: how to manage logs?
            thisRouteBuilder
                .MapGet("restartservice/{name}", context => new RouteImplementations().RestartService(context))
                .MapGet("killservice/{name}", context => new RouteImplementations().KillService(context))
                .MapGet("startservice/{name}", context => new RouteImplementations().StartService(context))
                .MapGet("restartpool/{name}", context => new RouteImplementations().RestartPool(context))
                .MapGet("startpool/{name}", context => new RouteImplementations().StartPool(context))
                .MapGet("killpool/{name}", context => new RouteImplementations().KillPool(context))
                .MapGet("restartserviceandpool/{servicename}/{applicationpoolname}", context => new RouteImplementations().RestartServiceAndPool(context));


        }
    }
}
