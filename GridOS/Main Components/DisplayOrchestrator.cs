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
        /// Main display component of GridOS. Builds and holds the full model of display data by exposing registration methods.
        /// Registers TextPanels, and knows how to instantiate all classes required for operating display content on TextPanels.
        /// </summary>
        class DisplayOrchestrator
        {
            // TODO: Now that the menu system is dynamic... need to think about whether we need to dispose stuff when an element is removed.
            // What to do if non-empty element is removed? Remove all children, or put them somewhere else? How to react if we're displaying elements below a group node that is removed?

            // Root group displayed by default, parent of all other groups
            private IDisplayGroup _displayRoot = new DisplayGroup("Main Menu");

            private List<IMyTextPanel> _registeredTextPanels = new List<IMyTextPanel>();
            private Dictionary<string, DisplayController> _namedControllers = new Dictionary<string, DisplayController>();

            // TODO: Totally redo command handling, get rid of multiple lookups in different classes
            private Dictionary<int, NavigationCommand> _commandMap = new Dictionary<int, NavigationCommand>()
            {
                { 2, NavigationCommand.Up },
                { 3, NavigationCommand.Down },
                { 4, NavigationCommand.Select }
            };

            public DisplayOrchestrator()
            {
                // Nothing to do here now
            }

            public void RegisterTextPanel(IMyTextPanel textPanel)
            {
                if (_registeredTextPanels.Contains(textPanel))
                    return;

                _registeredTextPanels.Add(textPanel);
                _namedControllers.Add("Display1", new DisplayController(new DisplayView(textPanel), new DisplayViewModel(_displayRoot)));
            }

            public void UnregisterTextPanel(IMyTextPanel textPanel)
            {
                if (!_registeredTextPanels.Contains(textPanel))
                    return;

                // TODO: Get rid of this hack, create proper removal infrastructure. CHECK REFERENCING to see if disposal is needed.
                int sharedIndex = _registeredTextPanels.IndexOf(textPanel);
                _namedControllers.Remove(_namedControllers.ElementAt(sharedIndex).Key);

                _registeredTextPanels.Remove(textPanel);
            }

            public void ProcessCommand(int numericalCommand)
            {
                if (!_commandMap.ContainsKey(numericalCommand))
                    return;

                // TODO: Currently doesn't support multiple controllers. Implement that functionality by changing the commanding structure too (nav. commands needs to be prefixed by controller name).
                _namedControllers.First().Value.ProcessCommand(_commandMap[numericalCommand]);
            }

            // TODO: Totally not good... move the responsibility of dealing with display elements directly into this class
            public void RegisterDisplayElement(IDisplayElement element)
            {
                _displayRoot.AddChild(element);
            }
        }
    }
}