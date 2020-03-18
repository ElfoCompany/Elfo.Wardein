using System;
using Elfo.Wardein.Integrations.Oracle.Integration;
using Elfo.Wardein.Oracle;
using Warden.Core;
using Warden.Integrations;

namespace Elfo.Wardein.Integrations
{
    public static class Extensions
    {
        public static WardenConfiguration.Builder IntegrateWithOracle(
            this WardenConfiguration.Builder builder,
            string connectionString,
            Action<OracleConnectionConfiguration.Builder> configurator = null)
        {
            builder.AddIntegration(OracleIntegration.Create(connectionString, configurator));

            return builder;
        }

        public static WardenConfiguration.Builder IntegrateWithOracle(
            this WardenConfiguration.Builder builder,
            OracleConnectionConfiguration configuration) 
            => builder.IntegrateWithOracle(OracleIntegration.Create(configuration));

        public static WardenConfiguration.Builder IntegrateWithOracle(
            this WardenConfiguration.Builder builder,
            OracleIntegration oracleIntegration)
        {
            builder.AddIntegration(oracleIntegration);

            return builder;
        }

        public static OracleIntegration Oracle(this IIntegrator integrator)
            => integrator.Resolve<OracleIntegration>();
    }
}
