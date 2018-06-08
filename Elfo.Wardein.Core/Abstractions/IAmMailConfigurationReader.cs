using Elfo.Wardein.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Abstractions
{
    public interface IAmMailConfigurationReader
    {
        MailConfiguration GetMailConfiguration();
    }
}
