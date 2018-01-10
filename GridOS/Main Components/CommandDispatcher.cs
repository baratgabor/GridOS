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

            public string ListCommands()
            {
                string s = "";
                foreach (var c in _commands)
                {
                    s += c.Key + Environment.NewLine;
                }
                return s;
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
            }
        }
    }
}