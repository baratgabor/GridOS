using System.Collections.Generic;
using System;

namespace IngameScript
{
    /// <summary>
    /// Responsible for registering lists of commands, and executing the appropriate method based on the command name supplied.
    /// </summary>
    class CommandDispatcher : ICommandDispatcher
    {
        private readonly ILogger _logger;
        private Dictionary<string, CommandItem> _commands = new Dictionary<string, CommandItem>();

        public CommandDispatcher(ILogger logger)
        {
            _logger = logger;
        }

        public ICommandDispatcher AddCommand(CommandItem command)
        {
            if (!_commands.ContainsValue(command))
            {
                _commands.Add(command.CommandName, command);
            }
            return this;
        }

        public void AddCommands(IEnumerable<CommandItem> commands)
        {
            foreach (CommandItem c in commands)
            {
                AddCommand(c);
            }
        }

        public void AddCommands_OverwriteExisting(IEnumerable<CommandItem> commands)
        {
            foreach (CommandItem c in commands)
            {
                _commands[c.CommandName] = c;
            }
        }

        public ICommandDispatcher AddCommand_OverwriteExisting(CommandItem command)
        {
            _commands[command.CommandName] = command;
            return this;
        }

        public string ListCommands()
        {
            string s = "";
            foreach (var c in _commands)
            {
                s += c.Key + Environment.NewLine;
            }
            return s;
        }

        public void RemoveCommand(CommandItem command)
        {
            _commands.Remove(command.CommandName);
        }

        public void RemoveCommands(IEnumerable<CommandItem> commands)
        {
            foreach (CommandItem c in commands)
            {
                _commands.Remove(c.CommandName);
            }
        }

        public void TryDispatch(string argument)
        {
            string commandName = "";
            string parameter = "";

            int commandEnd = argument.IndexOf(" ");

            // Single-word - only command
            if (commandEnd == -1)
            {
                commandName = argument;
                parameter = "";
            }
            // Multi-word - first word command, rest is parameter
            else
            {
                commandName = argument.Substring(0, commandEnd);
                parameter = argument.Substring(commandEnd).Trim();
            }

            CommandItem command;
            if (_commands.TryGetValue(commandName, out command))
            {
                command.Execute(command, parameter);
            }
            else
            {
                _logger.Log(LogLevel.Error, "Command Dispatcher has found no '{0}' command to execute. Original input argument: '{1}'", commandName, argument);
            }
        }
    }
}