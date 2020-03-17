using Elfo.Wardein.Abstractions.Services;
using Elfo.Wardein.Core.ExtensionMethods;
using Microsoft.Web.Administration;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elfo.Wardein.Core.ServiceManager
{
    public class IISPoolManager : IAmServiceManager
    {
        #region Private variables
        private readonly ApplicationPool applicationPool;
        private readonly static Logger log = LogManager.GetCurrentClassLogger();
        #endregion

        public IISPoolManager(string appPoolName) : base(appPoolName)
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
                log.Info($"Stopping pool {base.serviceName} @ {DateTime.UtcNow}");
                this.applicationPool.Stop();
                this.applicationPool.WaitForStatus(ObjectState.Stopped, TimeSpan.FromSeconds(30));
                log.Info($"{base.serviceName} pool stopped @ {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while killing {base.serviceName} IIS pool");
                throw;
            }
        }

        public override void Restart()
        {
            try
            {
                Stop();
                Start();
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while restarting {base.serviceName} IIS pool");
                throw;
            }
        }

        public override void Start()
        {
            try
            {
                log.Info($"Starting pool {base.serviceName} @ {DateTime.UtcNow}");
                if (!IsStillAlive)
                    this.applicationPool.Start();
                this.applicationPool.WaitForStatus(ObjectState.Started, TimeSpan.FromSeconds(30));
                log.Info($"{base.serviceName} pool started @ {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while starting {base.serviceName} IIS pool");
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
                log.Error(ex, $"Error while stopping {base.serviceName} IIS pool");
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
                log.Error(ex, $"Error while getting status for pool {base.serviceName} IIS pool");
                throw;
            }
        }
    }
}
