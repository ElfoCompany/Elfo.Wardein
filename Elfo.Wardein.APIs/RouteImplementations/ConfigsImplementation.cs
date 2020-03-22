using Elfo.Wardein.APIs.Abstractions;
using Elfo.Wardein.Core;
using Elfo.Wardein.Core.Helpers;
using Microsoft.AspNetCore.Http;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Elfo.Wardein.APIs
{
    public class ConfigsImplementation : IAmRouteImplementation
    {
        public ConfigsImplementation() : base(blockActionIfInMaintenanceMode: false) { }

        public async Task Invalidate(HttpContext context)
        {
            ServicesContainer.MailConfigurationManager().InvalidateCache();
            ServicesContainer.WardeinConfigurationManager().InvalidateCache();
            await new PollingImplementation().Restart(context);
            await context.Response.WriteAsync($"Cached configs invalidated");
        }
    }
}
