using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Abstractions
{
    public interface IAmBaseConfigurationManager<T> where T: new()
    {
        T GetConfiguration();

        void InvalidateCache();
    }
}
