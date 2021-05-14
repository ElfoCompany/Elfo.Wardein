using Elfo.Wardein.Abstractions.Configuration.Models;
using System.Threading.Tasks;

namespace Elfo.Wardein.Abstractions.BaseUrlWatcher
{
	public interface IAmUrlManager<T> where T : IAmConfigurationModelWithResolution
	{
		Task<bool> IsHealthy(T configuration);
		Task RestartPool(string poolName);
	}
}
