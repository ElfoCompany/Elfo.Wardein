using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Elfo.Wardein.Core.ExtensionMethods
{
    public static class ApplicationPoolExtensionMethods
    {
        public static void WaitForStatus(this ApplicationPool applicationPool, ObjectState desiredStatus) =>
            applicationPool.WaitForStatus(desiredStatus, TimeSpan.MaxValue);

        /// <summary>
        /// Waits until the service has reached the given status or until the specified time
        /// has expired
        /// </summary>
        public static void WaitForStatus(this ApplicationPool applicationPool, ObjectState desiredStatus, TimeSpan timeout)
        {
            if (!Enum.IsDefined(typeof(ObjectState), desiredStatus))
                throw new ArgumentException(string.Format(
                    "The value of argument '{0}' ({1}) is invalid for Enum type '{2}'.",
                    nameof(desiredStatus),
                    (int)desiredStatus,
                    typeof(ObjectState))
                );

            ManualResetEvent _waitForStatusSignal = new ManualResetEvent(false);

            DateTime start = DateTime.UtcNow;

            while (applicationPool.State != desiredStatus)
            {
                if (DateTime.UtcNow - start > timeout)
                    throw new System.ServiceProcess.TimeoutException($"App pool did not switched to desired status {desiredStatus} in a timely fashion");

                _waitForStatusSignal.WaitOne(250);
            }
        }
    }
}
