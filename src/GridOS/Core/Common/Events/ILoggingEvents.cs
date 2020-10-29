using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    public interface ILoggingEvents
    {
        event Action<string> ErrorLogged;
        event Action<string> WarningLogged;
        event Action<string> InformationLogged;
        event Action<string> DebugLogged;
    }
}
