using System;

namespace Elfo.Wardein.Core.Abstractions
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

        public abstract bool IsStillAlive { get; }
        public abstract void ForceKill();
        public abstract void Restart();
        public abstract void Start();
        public abstract void Stop();
        public abstract string GetStatus();
    }
}