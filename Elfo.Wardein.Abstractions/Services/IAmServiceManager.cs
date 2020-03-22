using System;
using System.Threading.Tasks;

namespace Elfo.Wardein.Abstractions.Services
{
    public abstract class IAmServiceManager
    {
        protected string serviceName;

        public IAmServiceManager(string serviceName)
        {
            #region Validations
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException("Service Name cannot be null");
            #endregion

            this.serviceName = serviceName;
        }

        public abstract Task<bool> IsStillAlive();
        public abstract Task ForceKill();
        public abstract Task Restart();
        public abstract Task Start();
        public abstract Task Stop();
        public abstract Task<string> GetStatus();
    }
}