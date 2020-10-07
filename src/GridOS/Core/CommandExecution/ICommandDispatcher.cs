using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Dispatches incoming command names to the appropriate registered commands, if any.
        /// </summary>
        interface ICommandDispatcher
        {
            /// <summary>
            /// Registers a new command.
            /// </summary>
            ICommandDispatcher AddCommand(CommandItem command);

            /// <summary>
            /// Registers a new command in a prioritised manner, replacing existing commands of the same name, if any.
            /// </summary>
            ICommandDispatcher AddCommand_OverwriteExisting(CommandItem command);

            /// <summary>
            /// Registers a list of new commands.
            /// </summary>
            void AddCommands(IEnumerable<CommandItem> commands);

            /// <summary>
            /// Registers a list of new commands in a prioritised manner, replacing existing commands of the same name, if any.
            /// </summary>
            void AddCommands_OverwriteExisting(IEnumerable<CommandItem> commands);

            /// <summary>
            /// Removes a command from the list of executable commands.
            /// </summary>
            void RemoveCommand(CommandItem command);

            /// <summary>
            /// Removes a list of commands.
            /// </summary>
            void RemoveCommands(IEnumerable<CommandItem> commands);

            /// <summary>
            /// Tries to dispatch a command line to the appropriate registered command, if any.
            /// On multi-word command lines, it assumes that the first word is the command name, and the rest are parameters.
            /// </summary>
            void TryDispatch(string commandLine);
        }
    }
}
