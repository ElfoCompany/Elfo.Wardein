using Elfo.Wardein.APIs.Abstractions;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PeterKottas.DotNetCore.WindowsService.Base;

namespace Elfo.Wardein.APIs
{
    public class PollingImplementation : IAmRouteImplementation
    {
        #region Private variables
        private readonly IMicroService microService;
        #endregion

        #region Constructor
        public PollingImplementation() : base(blockActionIfInMaintenanceMode: false)
        {
            this.microService = Startup.ServiceProvider.GetService<IMicroService>();
        }
        #endregion

        #region Public Methods

        public Task Stop(HttpContext context)
        {
            microService.Stop();
            return context.Response.WriteAsync($"Periodic check stopped");
        }

        public Task Restart(HttpContext context)
        {
            Stop(context);
            microService.Start();
            return context.Response.WriteAsync($"Periodic check restarted");
        } 

        #endregion
    }
}
