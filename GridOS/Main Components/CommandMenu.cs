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
        /// Represents a menu that displays only commands, and accepts menu navigation and menu item selection commands.
        /// </summary>
        class CommandMenu
        {
            private string CommandMenuString;
            public Action<string> MenuUpdateAction;

            private List<CommandItem> _commands = new List<CommandItem>();
            private Dictionary<int, Action> _navigationCommands = new Dictionary<int, Action>();
            private int _selectedCommandIndex = 0;

            private const string _menuTitle = "Command Menu";
            private const string _menuNavigationTip = "Up: 2, Down: 3, Select: 4";
            private const string _menuCommandNotSelectedPrefix = "   ";
            private const string _menuCommandSelectedPrefix = "-> ";

            // quick and dirty way to execute something on menu actions, e.g. playing a soundblock for adding a sound effect
            private Action _navigationActionHook;

            /// <summary>
            /// Creates new CommandMenu instance.
            /// </summary>
            /// <param name="navigationActionHook">An arbitrary method to call at each menu action, e.g. SoundBlock.Play() to add sound effects to the menu.</param>
            public CommandMenu(Action navigationActionHook)
            {
                _navigationActionHook = navigationActionHook;

                // hardcoding the menu nav. commands for now
                _navigationCommands[2] = MoveSelectionUp;
                _navigationCommands[3] = MoveSelectionDown;
                _navigationCommands[4] = ExecuteSelection;
            }

            /// <summary>
            /// Adds menu commands to the menu.
            /// </summary>
            /// <param name="commands">The list of commands to add to the menu.</param>
            public void AddCommands(List<CommandItem> commands)
            {
                foreach (CommandItem c in commands)
                {
                    if (!_commands.Contains(c))
                    {
                        _commands.Add(c);
                        c.PropertyChanged += HandleCommandPropertyChanges;
                    }
                }

                RenderMenu();
            }

            /// <summary>
            /// Handles incoming menu nagivation commands, e.g. moving selection and executing selected item.
            /// </summary>
            /// <param name="command">Numerical ID of command to execute.</param>
            public void ProcessNagivationCommand(int command)
            {
                if (_navigationCommands.ContainsKey(command))
                {
                    _navigationActionHook?.Invoke();
                    // TODO: probably (?) it would be neater not to execute here, but to send the command name to CommandDispatcher...
                    // (In that case this class doesn'T even need to store full command objects, possibly? Look into that)
                    // Although not sure adding that dependency here worth it; need to think about it
                    _navigationCommands[command].Invoke();
                }
            }

            /// <summary>
            /// Moves the item selection up.
            /// </summary>
            private void MoveSelectionUp()
            {
                if (_selectedCommandIndex == 0)
                    return;

                _selectedCommandIndex--;
                RenderMenu();
            }

            /// <summary>
            /// Moves the item selection down.
            /// </summary>
            private void MoveSelectionDown()
            {
                if (_selectedCommandIndex == _commands.Count - 1)
                    return;

                _selectedCommandIndex++;
                RenderMenu();
            }

            /// <summary>
            /// Executes the selected menu item.
            /// </summary>
            private void ExecuteSelection()
            {
                _commands[_selectedCommandIndex].Execute();
            }

            /// <summary>
            /// Renders the list of menu commands on TextPanel
            /// </summary>
            public void RenderMenu()
            {
                var sb = new StringBuilder();

                if (_menuTitle != "")
                {
                    sb.AppendLine(_menuTitle);
                    sb.AppendLine("-----------------------");
                }

                foreach (CommandItem c in _commands)
                {
                    if (_selectedCommandIndex == _commands.IndexOf(c))
                        sb.Append(_menuCommandSelectedPrefix);
                    else
                        sb.Append(_menuCommandNotSelectedPrefix);

                    sb.AppendLine(c.MenuDisplayName);
                }

                CommandMenuString = sb.ToString();
                MenuUpdateAction?.Invoke(CommandMenuString);
            }

            /// <summary>
            /// Handles the changes of menu command properties.
            /// </summary>
            private void HandleCommandPropertyChanges(CommandItem command)
            {
                // re-render menu on textpanel; nothing else is needed right now
                RenderMenu();
            }
        }
    }
}