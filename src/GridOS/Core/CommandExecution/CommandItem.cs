namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Defines a command name, and a method to execute when the given command is received.
        /// </summary>
        public class CommandItem
        {
            public string CommandName => _commandName;
            private string _commandName;
            public delegate void CommandEventHandler(
                CommandItem sender,
                string parameter
            );

            public CommandEventHandler Execute => _execute;
            private CommandEventHandler _execute;

            public CommandItem(string CommandName, CommandEventHandler Execute)
            {
                _commandName = CommandName;
                _execute = Execute;
            }
        }
    }
}