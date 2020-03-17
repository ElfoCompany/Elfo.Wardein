using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Abstractions.Configuration
{
    public interface IAmBaseConfigurationManager<T> where T: new()
    {
        T GetConfiguration();

        void InvalidateCache();
    }
}
