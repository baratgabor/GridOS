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
        /// Basic data object with properties representing a command that can be executed directly, or shown as menu commands.
        /// Contains logic for property changed notifications.
        /// </summary>
        public class CommandItem
        {
            // props
            private string _commandName;
            public string CommandName
            {
                get { return _commandName; }
                set { SetAndNotify(ref _commandName, value); }
            }

            private string _menuDisplayName;
            public string MenuDisplayName
            {
                get { return _menuDisplayName; }
                set { SetAndNotify(ref _menuDisplayName, value); }
            }

            private int _menuDisplayPriority;
            public int MenuDisplayPriority
            {
                get { return _menuDisplayPriority; }
                set { SetAndNotify(ref _menuDisplayPriority, value); }
            }

            private string _functionalityName;
            public string FunctionalityName
            {
                get { return _functionalityName; }
                set { SetAndNotify(ref _functionalityName, value); }
            }

            private Action _execute;
            public Action Execute
            {
                get { return _execute; }
                set { SetAndNotify(ref _execute, value); }
            }

            // constr
            public CommandItem(string CommandName, string MenuDisplayName, int MenuDisplayPriority, string FunctionalityName, Action Execute)
            {
                _commandName = CommandName;
                _menuDisplayName = MenuDisplayName;
                _menuDisplayPriority = MenuDisplayPriority;
                _functionalityName = FunctionalityName;
                _execute = Execute;
            }

            /// <summary>
            /// Provides property change notifications. Payload is the object itself whose property has been changed.
            /// </summary>
            public event Action<CommandItem> PropertyChanged;

            /// <summary>
            /// Sets value of field if it actually changed, and fires "propertychanged" event to notify subscribers
            /// </summary>
            /// <param name="field">Private field for which new value is assigned</param>
            /// <param name="value">New value to assign</param>
            /// <returns></returns>
            private void SetAndNotify<T>(ref T field, T value)
            {
                if (EqualityComparer<T>.Default.Equals(field, value))
                    return;

                field = value;
                PropertyChanged?.Invoke(this);
                return;
            }
        }
    }
}