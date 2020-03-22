using Elfo.Wardein.Core;
using Elfo.Wardein.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.APIs.Abstractions
{
    public abstract class IAmRouteImplementation
    {
        public IAmRouteImplementation(bool blockActionIfInMaintenanceMode)
        {
            if (blockActionIfInMaintenanceMode && ServicesContainer.WardeinConfigurationManager().IsInMaintenanceMode)
            {
                throw new InvalidOperationException("Wardein is in maintenance mode. Please try again later");
            }
        }
    }
}
