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
                StopService(serviceName);
                return context.Response.WriteAsync($"Service {serviceName} stopped");
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
                RestartApplicationPool(applicationPoolName);
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
                StartApplicationPool(applicationPoolName);
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
                KillApplicationPool(applicationPoolName);
                return context.Response.WriteAsync($"ApplicationPool {applicationPoolName} stopped");
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
        }

        public Task RestartServiceAndPool(HttpContext context)
        {
            string serviceName = context.GetRouteValue("servicename").ToString();
            string applicationPoolName = context.GetRouteValue("iispoolname").ToString();

            try
            {
                RestartService(serviceName);
                RestartApplicationPool(applicationPoolName);
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
        void StopService(string serviceName) => new WindowsServiceHelper(serviceName).ForceKill();
        void StartService(string serviceName) => new WindowsServiceHelper(serviceName).Start();
        void RestartApplicationPool(string applicationPoolName) => new IISPoolHelper(applicationPoolName).Restart();
        void KillApplicationPool(string applicationPoolName) => new IISPoolHelper(applicationPoolName).ForceKill();
        void StartApplicationPool(string applicationPoolName) => new IISPoolHelper(applicationPoolName).Start();

        #endregion
    }
}
