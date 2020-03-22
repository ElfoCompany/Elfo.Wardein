using Elfo.Wardein.APIs.Abstractions;
using Elfo.Wardein.Core;
using Elfo.Wardein.Core.ServiceManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
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

            var status = await GetServiceStatus(serviceName);
            return status; // TODO: print on webpage
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

            var status = await GetIISPoolStatus(applicationPoolName);
            return status; // TODO: print in webpage
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
