using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Responsible for registering lists of commands, and executing the appropriate method based on the command name supplied.
        /// </summary>
        class CommandDispatcher : ICommandDispatcher
        {
            private Dictionary<string, CommandItem> _commands = new Dictionary<string, CommandItem>();

            public void AddCommands(List<CommandItem> commands)
            {
                foreach (CommandItem c in commands)
                {
                    if (!_commands.ContainsValue(c))
                    {
                        _commands.Add(c.CommandName, c);
                    }
                }
            }

            public void AddCommands_OverwriteExisting(List<CommandItem> commands)
            {
                foreach (CommandItem c in commands)
                {
                    _commands[c.CommandName] = c;
                }
            }

            public void RemoveCommands(List<CommandItem> commands)
            {
                foreach (CommandItem c in commands)
                {
                    if (_commands.ContainsValue(c))
                    {
                        _commands.Remove(c.CommandName);
                    }
                }
            }

            public void TryDispatch(string argument)
            {
                // Isolate first word as command; return if not successful
                int commandEnd = argument.Trim().IndexOf(" ");
                if (commandEnd == -1)
                    return;

                // Use first word as command, and the rest as parameter
                string commandName = argument.Substring(0, commandEnd);
                string parameter = argument.Substring(commandEnd).Trim();

                CommandItem command;
                if (_commands.TryGetValue(commandName, out command))
                {
                    command.Execute(command, parameter);
                }
            }
        }
    }
}