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
        /// Main display component of GridOS, responsible for abstracting display content by registering various displayable components, and maintaining state about which one is displayed.
        /// Currently limited, only supports CommandMenu component.
        /// </summary>
        class GridOSDisplay
        {
            // Has a CommandMenu, or possibly multiple
            // Has a TextPanel, or possibly multiple, for showing actual system screen content
            // In future, it could have different screen content, not just the command menu

            // Simple numerical arguments sent to the PB (1,2,3,4,5) deemed to be system interaction commands,
            // and these need to be sent to this class to ascertain the context of the commands (e.g. what's shown on screen),
            // and send the commands to the proper place

            private CommandMenu _commandMenu;
            private List<IMyTextPanel> _textPanels = new List<IMyTextPanel>();

            public GridOSDisplay(CommandMenu commandMenu)
            {
                _commandMenu = commandMenu;
                _commandMenu.MenuUpdateAction = UpdateScreens;
            }

            public void UpdateScreens(string menuContent)
            {
                if (_textPanels.Count == 0)
                    return;

                for (int x = 0; x < _textPanels.Count; x++)
                {
                    try
                    {
                        _textPanels[x].WritePublicText(menuContent);
                    }
                    catch
                    {
                        // TextPanel probably dead ?
                        UnregisterTextPanel(_textPanels[x]);
                    }
                }
            }

            public void RegisterTextPanel(IMyTextPanel textPanel)
            {
                if (_textPanels.Contains(textPanel))
                    return;

                _textPanels.Add(textPanel);
                _commandMenu.RenderMenu();

            }

            public void UnregisterTextPanel(IMyTextPanel textPanel)
            {
                if (!_textPanels.Contains(textPanel))
                    return;

                _textPanels.Remove(textPanel);
            }

            public void ProcessDisplayCommand(int numericalCommand)
            {
                _commandMenu.ProcessNagivationCommand(numericalCommand);
            }

            public void AddCommands(List<CommandItem> commands)
            {
                _commandMenu.AddCommands(commands);
            }
        }
    }
}