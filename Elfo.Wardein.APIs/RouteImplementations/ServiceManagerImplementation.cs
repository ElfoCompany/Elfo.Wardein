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

        public Task KillService(HttpContext context)
        {
            string serviceName = context.GetRouteValue("name").ToString();

            KillService(serviceName);
            return context.Response.WriteAsync($"Service {serviceName} stopped");
        }

        public Task GetServiceStatus(HttpContext context)
        {
            string serviceName = context.GetRouteValue("name").ToString();

            var status = GetServiceStatus(serviceName);
            return context.Response.WriteAsync(status);
        }

        public Task StartService(HttpContext context)
        {
            string serviceName = context.GetRouteValue("name").ToString();

            StartService(serviceName);
            return context.Response.WriteAsync($"Service {serviceName} started");
        }
        #endregion

        #region IIS Pool management
        public Task RestartPool(HttpContext context)
        {
            string applicationPoolName = context.GetRouteValue("name").ToString();

            RestartIISPool(applicationPoolName);
            return context.Response.WriteAsync($"ApplicationPool {applicationPoolName} restarted");
        }
        public Task StartPool(HttpContext context)
        {
            string applicationPoolName = context.GetRouteValue("name").ToString();

            StartIISPool(applicationPoolName);
            return context.Response.WriteAsync($"ApplicationPool {applicationPoolName} started");
        }

        public Task KillPool(HttpContext context)
        {
            string applicationPoolName = context.GetRouteValue("name").ToString();

            KillIISPool(applicationPoolName);
            return context.Response.WriteAsync($"ApplicationPool {applicationPoolName} stopped");
        }

        public Task GetPoolStatus(HttpContext context)
        {
            string applicationPoolName = context.GetRouteValue("name").ToString();

            var status = GetIISPoolStatus(applicationPoolName);
            return context.Response.WriteAsync(status);
        }

        #endregion

        #region Windows Service + IIS Pool management

        public Task RestartServiceAndPool(HttpContext context)
        {
            string serviceName = context.GetRouteValue("servicename").ToString();
            string applicationPoolName = context.GetRouteValue("iispoolname").ToString();

            RestartService(serviceName);
            RestartIISPool(applicationPoolName);
            return context.Response.WriteAsync($"Service {serviceName} restarted, applicationPool {applicationPoolName} restarted");
        }

        #endregion

        #region Local Functions

        void RestartService(string serviceName) => new WindowsServiceHelper(serviceName).Restart();
        void KillService(string serviceName) => new WindowsServiceHelper(serviceName).ForceKill();
        void StartService(string serviceName) => new WindowsServiceHelper(serviceName).Start();
        string GetServiceStatus(string serviceName) => new WindowsServiceHelper(serviceName).GetStatus();
        void RestartIISPool(string iisPoolName) => new IISPoolHelper(iisPoolName).Restart();
        void KillIISPool(string issPoolName) => new IISPoolHelper(issPoolName).ForceKill();
        string GetIISPoolStatus(string iisPoolName) => new IISPoolHelper(iisPoolName).GetStatus();
        void StartIISPool(string iisPoolName) => new IISPoolHelper(iisPoolName).Start();

        #endregion
    }
}
