using Elfo.Wardein.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Wardein.APIs
{
    public class RouteImplementations
    {
        #region Windows Service management
        public Task RestartService(HttpContext context)
        {
            string serviceName = context.GetRouteValue("name").ToString();

            try
            {
                RestartService(serviceName);
                return context.Response.WriteAsync($"Service {serviceName} restarted");
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
        }

        public Task KillService(HttpContext context)
        {
            string serviceName = context.GetRouteValue("name").ToString();

            try
            {
                KillService(serviceName);
                return context.Response.WriteAsync($"Service {serviceName} stopped");
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
        }

        public Task GetServiceStatus(HttpContext context)
        {
            string serviceName = context.GetRouteValue("name").ToString();

            try
            {
                var status = GetServiceStatus(serviceName);
                return context.Response.WriteAsync(status);
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
        }

        public Task StartService(HttpContext context)
        {
            string serviceName = context.GetRouteValue("name").ToString();

            try
            {
                StartService(serviceName);
                return context.Response.WriteAsync($"Service {serviceName} started");
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
        }
        #endregion

        #region IIS Pool management
        public Task RestartPool(HttpContext context)
        {            
            string applicationPoolName = context.GetRouteValue("name").ToString();

            try
            {
                RestartIISPool(applicationPoolName);
                return context.Response.WriteAsync($"ApplicationPool {applicationPoolName} restarted");
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
        }
        public Task StartPool(HttpContext context)
        {
            string applicationPoolName = context.GetRouteValue("name").ToString();

            try
            {
                StartIISPool(applicationPoolName);
                return context.Response.WriteAsync($"ApplicationPool {applicationPoolName} started");
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
        }

        public Task KillPool(HttpContext context)
        {
            string applicationPoolName = context.GetRouteValue("name").ToString();

            try
            {
                KillIISPool(applicationPoolName);
                return context.Response.WriteAsync($"ApplicationPool {applicationPoolName} stopped");
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
        }

        public Task GetPoolStatus(HttpContext context)
        {
            string applicationPoolName = context.GetRouteValue("name").ToString();

            try
            {
                var status = GetIISPoolStatus(applicationPoolName);
                return context.Response.WriteAsync(status);
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
        }

        #endregion

        #region Windows Service + IIS Pool management

        public Task RestartServiceAndPool(HttpContext context)
        {
            string serviceName = context.GetRouteValue("servicename").ToString();
            string applicationPoolName = context.GetRouteValue("iispoolname").ToString();

            try
            {
                RestartService(serviceName);
                RestartIISPool(applicationPoolName);
                return context.Response.WriteAsync($"Service {serviceName} restarted, applicationPool {applicationPoolName} restarted");
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
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
