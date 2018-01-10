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
        /// Defines a command name, and a method to execute when the given command is received.
        /// </summary>
        public class CommandItem
        {
            public string CommandName => _commandName;
            private string _commandName;

            public Action<CommandItem> Execute => _execute;
            private Action<CommandItem> _execute;

            public CommandItem(string CommandName, Action<CommandItem> Execute)
            {
                _commandName = CommandName;
                _execute = Execute;
            }
        }
    }
}