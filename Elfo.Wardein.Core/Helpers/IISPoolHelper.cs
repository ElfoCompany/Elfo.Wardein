using Elfo.Wardein.Core.Abstractions;
using Elfo.Wardein.Core.ExtensionMethods;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elfo.Wardein.Core.Helpers
{
    public class IISPoolHelper : IAmServiceManager
    {
        #region Private variables
        private readonly ApplicationPool applicationPool;
        #endregion

        public IISPoolHelper(string appPoolName) : base(appPoolName)
        {
            this.applicationPool = new ServerManager().ApplicationPools.FirstOrDefault(x => x.Name == appPoolName);

            if (this.applicationPool == null)
                throw new ArgumentNullException($"ApplicationPool {appPoolName} doesn't exist");
        }

        public override bool IsStillAlive => this.applicationPool.State == ObjectState.Started;

        public override void ForceKill()
        {
            try
            {
                Console.WriteLine($"Stopping pool {base.serviceName} @ {DateTime.UtcNow}");
                this.applicationPool.Stop();
                this.applicationPool.WaitForStatus(ObjectState.Stopped, TimeSpan.FromSeconds(30));
                Console.WriteLine($"{base.serviceName} pool stopped @ {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while killing {base.serviceName} IIS pool: {ex.Message}");
                throw;
            }
        }

        public override void Restart()
        {
            try
            {
                ForceKill();
                Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while restarting {base.serviceName} IIS pool: {ex.Message}");
                throw;
            }
        }

        public override void Start()
        {
            try
            {
                Console.WriteLine($"Starting pool {base.serviceName} @ {DateTime.UtcNow}");
                if (!IsStillAlive)
                    this.applicationPool.Start();
                this.applicationPool.WaitForStatus(ObjectState.Started, TimeSpan.FromSeconds(30));
                Console.WriteLine($"{base.serviceName} pool started @ {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while starting {base.serviceName} IIS pool: {ex.Message}");
                throw;
            }
        }

        public override void Stop()
        {
            try
            {
                if (IsStillAlive)
                    this.ForceKill();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while stopping {base.serviceName} IIS pool: {ex.Message}");
                throw;
            }
        }

        public override string GetStatus()
        {
            try
            {
                return this.applicationPool.State.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while getting status for pool {base.serviceName} IIS pool: {ex.Message}");
                throw;
            }
        }
    }
}
