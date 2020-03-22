using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elfo.Wardein.Abstractions
{
    public enum NotificationType
    {
        [Description("Mail")]
        Mail = 0,
        [Description("Teams")]
        Teams = 1
    }
}
