using Elfo.Wardein.APIs.Abstractions;
using Elfo.Wardein.Core;
using Elfo.Wardein.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Wardein.APIs
{
    public class MaintenanceImplementation : IAmRouteImplementation
    {
        public MaintenanceImplementation() : base(blockActionIfInMaintenanceMode: false) { }

        #region Maintenance Mode

        public Task StartMaintenanceMode(HttpContext context)
        {
            if (!double.TryParse(context.GetRouteData().Values["durationInSeconds"]?.ToString(), out double durationInSecond))
                durationInSecond = TimeSpan.FromMinutes(5).TotalSeconds; // Default value

            ServicesContainer.WardeinConfigurationManager().StartMaintenanceMode(durationInSecond);
            return context.Response.WriteAsync($"Maintenance Mode Started");
        }

        public Task StopMaintenanceMode(HttpContext context)
        {
            ServicesContainer.WardeinConfigurationManager().StopMaintenaceMode();
            return context.Response.WriteAsync($"Maintenance Mode Stopped");
        }

        public Task GetMaintenanceModeStatus(HttpContext context)
        {
            var result = "Wardein is not in maintenance mode";
            if (ServicesContainer.WardeinConfigurationManager().IsInMaintenanceMode)
                result = "Wardein is in maintenance mode";
            return context.Response.WriteAsync(result);
        }
        #endregion
    }
}
