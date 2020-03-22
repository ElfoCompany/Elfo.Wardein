using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Wardein.Abstractions.HeartBeat
{
    public interface IAmWardeinHeartBeatPersistanceService
    {
        Task<DateTime> GetLastHearBeat(string applicationHostname);
        Task<bool> UpdateHeartBeat(string applicationHostname);
    }
}
