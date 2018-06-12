using Microsoft.AspNetCore.Routing;

namespace Elfo.Wardein.APIs
{
    public static class RouteBuilderExtensionMethods
    {
        public static void ConfigureWardeinRouting(this RouteBuilder thisRouteBuilder)
        {
            //TODO: how to manage logs?
            thisRouteBuilder
                .MapGet("api/1.0/ws/restart/{name}", context => new RouteImplementations().RestartService(context))
                .MapGet("api/1.0/ws/kill/{name}", context => new RouteImplementations().KillService(context))
                .MapGet("api/1.0/ws/start/{name}", context => new RouteImplementations().StartService(context))
                .MapGet("api/1.0/pool/restart/{name}", context => new RouteImplementations().RestartPool(context))
                .MapGet("api/1.0/pool/start/{name}", context => new RouteImplementations().StartPool(context))
                .MapGet("api/1.0/pool/kill/{name}", context => new RouteImplementations().KillPool(context))
                .MapGet("api/1.0/wspool/restart/{servicename}/{applicationpoolname}", context => new RouteImplementations().RestartServiceAndPool(context));


        }
    }
}
