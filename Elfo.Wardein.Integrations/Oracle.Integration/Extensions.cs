using System;
using Elfo.Wardein.Integrations.Oracle.Integration;
using Warden.Core;
using Warden.Integrations;

namespace Elfo.Wardein.Integrations
{
    public static class Extensions
    {
        public static WardenConfiguration.Builder IntegrateWithOracle(
            this WardenConfiguration.Builder builder,
            string connectionString,
            Action<OracleIntegrationConfiguration.Builder> configurator = null)
        {
            builder.AddIntegration(OracleIntegration.Create(connectionString, configurator));

            return builder;
        }

        public static WardenConfiguration.Builder IntegrateWithOracle(
            this WardenConfiguration.Builder builder,
            OracleIntegrationConfiguration configuration)
        {
            builder.AddIntegration(OracleIntegration.Create(configuration));

            return builder;
        }

        public static OracleIntegration Oracle(this IIntegrator integrator)
            => integrator.Resolve<OracleIntegration>();
    }
}
