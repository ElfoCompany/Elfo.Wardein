using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Abstractions.Watchers
{
    public class WatcherStatusResult
    {
        public bool PreviousStatus { get; set; }
        public int FailureCount { get; set; }
    }
}
