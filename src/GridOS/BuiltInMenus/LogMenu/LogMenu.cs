using System;

namespace IngameScript
{
    class LogMenu : MenuGroup
    {

        public LogMenu(ILoggingEvents loggingEvents) : base("Log")
        {
            AddChild(new MenuItem("Events logged since last restart."));
            AddChild(new LogList(loggingEvents, LogLevel.Error));
            AddChild(new LogList(loggingEvents, LogLevel.Warning));
            AddChild(new LogList(loggingEvents, LogLevel.Information));
            AddChild(new CommandList());
        }
    }


    class CommandList : MenuGroup
    {
        public CommandList() : base("Commands Available: {}")
        {

        }
    }
}
