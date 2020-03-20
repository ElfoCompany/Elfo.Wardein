using System;
using System.Threading.Tasks;

namespace Elfo.Firmenich.Wardein.Abstractions.WebWatcher
{
    public interface IAmUrlResponseManager
    {
        Task<bool> IsHealthy(bool assertWithStatusCode, string assertWithRegex, Uri url);
        Task RestartPool(string poolName);
    }
}
