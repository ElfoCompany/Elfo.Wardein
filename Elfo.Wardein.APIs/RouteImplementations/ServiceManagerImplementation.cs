using Elfo.Wardein.APIs.Abstractions;
using Elfo.Wardein.Core;
using Elfo.Wardein.Core.ServiceManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace Elfo.Wardein.APIs
{
    public class ServiceManagerImplementation : IAmRouteImplementation
    {
        public ServiceManagerImplementation() : base(blockActionIfInMaintenanceMode: true) { }

        #region Windows Service management
        public Task RestartService(HttpContext context)
        {
            string serviceName = context.GetRouteValue("name").ToString();

            RestartService(serviceName);
            return context.Response.WriteAsync($"Service {serviceName} restarted");
        }

        public Task StopService(HttpContext context)
        {
            string serviceName = context.GetRouteValue("name").ToString();

            StopService(serviceName);
            return context.Response.WriteAsync($"Service {serviceName} stopped");
        }

        public async Task<string> GetServiceStatus(HttpContext context)
        {
            string serviceName = context.GetRouteValue("name").ToString();

            var result = $"{serviceName} is not running";

            var status = await GetServiceStatus(serviceName);
            if (status == "Running")
            {
                result = $"{serviceName} is running";
            }    
            return context.Response.WriteAsync(result).ToString();
        }

        public Task GetMaintenanceModeStatus(HttpContext context)
        {
            var result = "Wardein is not in maintenance mode";
            if (ServicesContainer.WardeinConfigurationManager().IsInMaintenanceMode)
                result = "Wardein is in maintenance mode";
            return context.Response.WriteAsync(result);
        }

        public Task StartService(HttpContext context)
        {
            string serviceName = context.GetRouteValue("name").ToString();

            StartService(serviceName);
            return context.Response.WriteAsync($"Service {serviceName} started");
        }
        #endregion

        #region IIS Pool management
        public Task RefreshPool(HttpContext context)
        {
            string applicationPoolName = context.GetRouteValue("name").ToString();

            RefreshIISPool(applicationPoolName);
            return context.Response.WriteAsync($"ApplicationPool {applicationPoolName} restarted");
        }
        public Task StartPool(HttpContext context)
        {
            string applicationPoolName = context.GetRouteValue("name").ToString();

            StartIISPool(applicationPoolName);
            return context.Response.WriteAsync($"ApplicationPool {applicationPoolName} started");
        }

        public Task StopPool(HttpContext context)
        {
            string applicationPoolName = context.GetRouteValue("name").ToString();

            StopIISPool(applicationPoolName);
            return context.Response.WriteAsync($"ApplicationPool {applicationPoolName} stopped");
        }

        public async Task<string> GetPoolStatus(HttpContext context)
        {
            string applicationPoolName = context.GetRouteValue("name").ToString();
           
            var result = $"{applicationPoolName} is not running";
            
            var status = await GetIISPoolStatus(applicationPoolName);
            if (status == "Started")
            {
                result = $"{applicationPoolName} is running";
            }
            return context.Response.WriteAsync(result).ToString();
        }

        #endregion

        #region Windows Service + IIS Pool management

        public Task RestartServiceAndPool(HttpContext context)
        {
            string serviceName = context.GetRouteValue("servicename").ToString();
            string applicationPoolName = context.GetRouteValue("iispoolname").ToString();

            RestartService(serviceName);
            RefreshIISPool(applicationPoolName);
            return context.Response.WriteAsync($"Service {serviceName} restarted, applicationPool {applicationPoolName} restarted");
        }

        #endregion

        #region Local Functions

        async Task RestartService(string serviceName) => await new WindowsServiceManager(serviceName).Restart();
        async Task StopService(string serviceName) => await new WindowsServiceManager(serviceName).Stop();
        async Task StartService(string serviceName) => await new WindowsServiceManager(serviceName).Start();
        async Task<string> GetServiceStatus(string serviceName) => await new WindowsServiceManager(serviceName).GetStatus();
        async Task RefreshIISPool(string iisPoolName) => await new IISPoolManager(iisPoolName).Restart();
        async Task StopIISPool(string issPoolName) => await new IISPoolManager(issPoolName).Stop();
        async Task<string> GetIISPoolStatus(string iisPoolName) => await new IISPoolManager(iisPoolName).GetStatus();
        async Task StartIISPool(string iisPoolName) => await new IISPoolManager(iisPoolName).Start();

        #endregion
    }
}
