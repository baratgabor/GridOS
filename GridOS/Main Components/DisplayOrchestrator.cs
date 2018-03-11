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
            private IDisplayGroup _displayRoot = new DisplayGroup("Main");

            private List<IMyTextPanel> _registeredTextPanels = new List<IMyTextPanel>();
            private List<DisplayController> _controllers = new List<DisplayController>();
            private const string _controllerNameTemplate = "Display";
            private int _controllerCounter = 0;

            private ICommandDispatcher _commandDispatcher;
            private IMyGridProgramRuntimeInfo _runtime;

            public DisplayOrchestrator(ICommandDispatcher commandDispatcher, IMyGridProgramRuntimeInfo runtime)
            {
                _commandDispatcher = commandDispatcher;
                _runtime = runtime;

                _displayRoot.AddChild(new HelpMenu());
            }

            public void RegisterTextPanel(IMyTextPanel textPanel)
            {
                if (_registeredTextPanels.Contains(textPanel))
                    return;

                try
                {
                    _controllers.Add(
                    new DisplayController(
                        NextControllerName(),
                        _commandDispatcher,
                        new DisplayView(textPanel, _runtime),
                        new DisplayViewModel(_displayRoot),
                        _runtime)
                    );
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + "\n" + e.StackTrace);
                }

                _registeredTextPanels.Add(textPanel);
            }

            public void UnregisterTextPanel(IMyTextPanel textPanel)
            {
                if (!_registeredTextPanels.Contains(textPanel))
                    return;

                // TODO: Create proper removal infrastructure. CHECK REFERENCING to see if disposal is needed.
                int sharedIndex = _registeredTextPanels.IndexOf(textPanel);
                _controllers.Remove(_controllers[sharedIndex]);

                _registeredTextPanels.Remove(textPanel);
            }

            // TODO: Totally not good... move the responsibility of dealing with display elements directly into this class
            public void RegisterDisplayElement(IDisplayElement element)
            {
                _displayRoot.AddChild(element);
            }

            private string NextControllerName()
            {
                return $"{_controllerNameTemplate}{++_controllerCounter}";
            }

            public void ClearAll()
            {
                _registeredTextPanels.ForEach(x => x.WritePublicText(""));
            }
        }
    }
}