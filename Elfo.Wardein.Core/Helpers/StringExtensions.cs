using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elfo.Wardein.Core.Helpers
{
    public static class StringExtensions
    {
        public static string Repeat(this string source, int times)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return string.Concat(Enumerable.Repeat(source, times));
        }
    }
}
