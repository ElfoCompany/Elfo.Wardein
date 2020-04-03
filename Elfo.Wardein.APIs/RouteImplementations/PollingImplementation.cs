using Elfo.Wardein.APIs.Abstractions;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PeterKottas.DotNetCore.WindowsService.Base;
using Warden;

namespace Elfo.Wardein.APIs
{
    public class PollingImplementation : IAmRouteImplementation
    {
        #region Private variables
        private readonly IWarden wardenInstance;
        #endregion

        #region Constructor
        public PollingImplementation() : base(blockActionIfInMaintenanceMode: false)
        {
            this.wardenInstance = Startup.ServiceProvider.GetService<IWarden>();
        }
        #endregion

        #region Public Methods

        public async Task Stop(HttpContext context)
        {
            await wardenInstance.StopAsync();
            await context.Response.WriteAsync($"Periodic check stopped");
        }

        public async Task Restart(HttpContext context)
        {
            await Stop(context);
            wardenInstance.StartAsync();            
            await context.Response.WriteAsync($"Periodic check restarted");
        } 

        #endregion
    }
}
