﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace Elfo.Wardein.APIs
{
    public static class RouteBuilderExtensionMethods
    {
        public static void ConfigureWardeinRouting(this RouteBuilder thisRouteBuilder)
        {
            //TODO: how to manage logs?
            thisRouteBuilder
                /************************************* WINDOWS SERVICE **********************************************************/
                .MapGet("api/1.0/status", ctx => ctx.Response.WriteAsync($"Wardein APIs are available"))
                /************************************* WINDOWS SERVICE **********************************************************/
                .MapGet("api/1.0/ws/restart/{name}", ctx => ctx.ApiTryCatch(() => new ServiceManagerImplementation().RestartService(ctx)))
                .MapGet("api/1.0/ws/stop/{name}", ctx => ctx.ApiTryCatch(() => new ServiceManagerImplementation().StopService(ctx)))
                .MapGet("api/1.0/ws/start/{name}", ctx => ctx.ApiTryCatch(() => new ServiceManagerImplementation().StartService(ctx)))
                .MapGet("api/1.0/ws/status/{name}", ctx => ctx.ApiTryCatch(() => new ServiceManagerImplementation().GetServiceStatus(ctx)))
                /**************************************** IIS POOL **************************************************************/
                .MapGet("api/1.0/pool/refresh/{name}", ctx => ctx.ApiTryCatch(() => new ServiceManagerImplementation().RefreshPool(ctx)))
                .MapGet("api/1.0/pool/start/{name}", ctx => ctx.ApiTryCatch(() => new ServiceManagerImplementation().StartPool(ctx)))
                .MapGet("api/1.0/pool/stop/{name}", ctx => ctx.ApiTryCatch(() => new ServiceManagerImplementation().StopPool(ctx)))
                .MapGet("api/1.0/pool/status/{name}", ctx => ctx.ApiTryCatch(() => new ServiceManagerImplementation().GetPoolStatus(ctx)))
                /*********************************** WINDOWS SERVICE + IIS POOL *************************************************/
                .MapGet("api/1.0/wspool/restart/{servicename}/{iispoolname}", ctx => ctx.ApiTryCatch(() => new ServiceManagerImplementation().RestartServiceAndPool(ctx)))
                /**************************************** MAINTENANCE MODE ******************************************************/
                .MapGet("api/1.0/maintenance/start/{durationInSeconds?}", ctx => ctx.ApiTryCatch(() => new MaintenanceImplementation().StartMaintenanceMode(ctx)))
                .MapGet("api/1.0/maintenance/stop", ctx => ctx.ApiTryCatch(() => new MaintenanceImplementation().StopMaintenanceMode(ctx)))
                .MapGet("api/1.0/maintenance/status", ctx => ctx.ApiTryCatch(() => new MaintenanceImplementation().GetMaintenanceModeStatus(ctx)))
                /**************************************** CONFIGS ******************************************************/
                .MapGet("api/1.0/configs/invalidate", ctx => ctx.ApiTryCatch(() => new ConfigsImplementation().Invalidate(ctx)))
                /**************************************** POLLING ******************************************************/
                .MapGet("api/1.0/polling/stop", ctx => ctx.ApiTryCatch(() => new PollingImplementation().Stop(ctx)))
                .MapGet("api/1.0/polling/restart", ctx => ctx.ApiTryCatch(() => new PollingImplementation().Restart(ctx)));
        }
    }
}
