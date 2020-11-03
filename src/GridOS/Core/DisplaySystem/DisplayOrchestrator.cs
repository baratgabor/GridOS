using System.Collections.Generic;
using System;

namespace IngameScript
{
    /// <summary>
    /// Main display component of GridOS. Builds and holds the full model of display data by exposing registration methods.
    /// Registers TextSurfaces, and knows how to instantiate all classes required for operating display content on TextSurfaces.
    /// </summary>
    class DisplayOrchestrator
    {
        // TODO: Now that the menu system is dynamic... need to think about whether we need to dispose stuff when an item is removed.
        // What to do if non-empty item is removed? Remove all children, or put them somewhere else? How to react if we're displaying items below a group node that is removed?

        // Root group displayed by default, parent of all other items/groups
        private readonly IMenuGroup _menuRoot = new MenuGroup("Main");

        private readonly List<IMyTextSurface> _registeredTextSurfaces = new List<IMyTextSurface>();
        private readonly List<DisplayController> _controllers = new List<DisplayController>();
        private const string _controllerNameTemplate = "Display";
        private int _controllerCounter = 0;

        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IDiagnosticService _diagnostics;
        private readonly IGlobalEvents _globalEvents;

        public DisplayOrchestrator(ICommandDispatcher commandDispatcher, IDiagnosticService diagnostics, IGlobalEvents globalEvents)
        {
            _commandDispatcher = commandDispatcher;
            _diagnostics = diagnostics;
            _globalEvents = globalEvents;
        }

        public void RegisterTextSurface(IMyTextSurface textSurface)
        {
            if (_registeredTextSurfaces.Contains(textSurface))
                return;

            try
            {
                var config = new MainConfig();

                _controllers.Add(
                new DisplayController(
                    NextControllerName(),
                    _commandDispatcher,
                    config,
                    _diagnostics,
                    new DisplayView(
                        textSurface,
                        config,
                        new TextSurfaceWordWrapper()),
                    _menuRoot,
                    _globalEvents)
                );
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n" + e.StackTrace);
            }

            _registeredTextSurfaces.Add(textSurface);
        }

        public void UnregisterTextSurface(IMyTextSurface textSurface)
        {
            if (!_registeredTextSurfaces.Contains(textSurface))
                return;

            // TODO: Create proper removal infrastructure. CHECK REFERENCING to see if disposal is needed.
            int sharedIndex = _registeredTextSurfaces.IndexOf(textSurface);
            _controllers.Remove(_controllers[sharedIndex]);

            _registeredTextSurfaces.Remove(textSurface);
        }

        public void RegisterMenuItem(IMenuItem item)
        {
            _menuRoot.AddChild(item);
        }

        private string NextControllerName()
        {
            return $"{_controllerNameTemplate}{++_controllerCounter}";
        }

        public void ClearAll()
        {
            _registeredTextSurfaces.ForEach(x => x.WriteText(""));
        }
    }
}