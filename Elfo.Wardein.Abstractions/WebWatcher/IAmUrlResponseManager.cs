using System.Threading.Tasks;

namespace Elfo.Firmenich.Wardein.Abstractions.WebWatcher
{
    public interface IAmUrlResponseManager
    {
        Task<bool> IsHealty(bool assertWithStatusCode, string assertWithRegex);
        Task RestartPool(string poolName);
    }
}
