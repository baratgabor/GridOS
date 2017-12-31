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
            // TODO: This class is extremely similar to the CommandMenu class... see if it's possible to structure them better to avoid repetitious behavior implementation
            private Dictionary<string, CommandItem> _commands = new Dictionary<string, CommandItem>();

            public void AddCommands(List<CommandItem> commands)
            {
                foreach (CommandItem c in commands)
                {
                    if (!_commands.ContainsValue(c))
                    {
                        _commands.Add(c.CommandName, c);
                        //c.PropertyChanged += HandleCommandPropertyChanges;
                    }
                }
            }

            public void RemoveCommands(List<CommandItem> commands)
            {
                foreach (CommandItem c in commands)
                {
                    if (_commands.ContainsValue(c))
                    {
                        //c.PropertyChanged -= HandleCommandPropertyChanges;
                        _commands.Remove(c.CommandName);
                    }
                }
            }

            public bool TryDispatch(string commandName)
            {
                if (!_commands.ContainsKey(commandName))
                    return false;

                _commands[commandName].Execute();
                return true;
            }
        }
    }
}