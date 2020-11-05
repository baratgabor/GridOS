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
        private const string _controllerNameTemplate = "Lcd";

        // All menu content.
        private readonly IMenuGroup _menuRoot = new MenuGroup("Main");

        // All displays used by the system, along with the corresponding controller.
        private readonly Dictionary<IMyTextSurface, DisplayController> _registeredSurfaces = new Dictionary<IMyTextSurface, DisplayController>();
        
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IGlobalEvents _globalEvents;
        private readonly ILogger _logger;
        private int _controllerCounter = 0;

        public DisplayOrchestrator(ICommandDispatcher commandDispatcher, IGlobalEvents globalEvents, ILogger logger)
        {
            _commandDispatcher = commandDispatcher;
            _globalEvents = globalEvents;
            _logger = logger;
        }

        public void RegisterTextSurface(IMyTextSurface textSurface)
        {
            if (_registeredSurfaces.ContainsKey(textSurface))
                return;

            DisplayController controller = null;

            try
            {
                var instanceConfig = new BaseConfig
                {
                    DisplayId = NextControllerName()
                };

                controller = new DisplayController(
                    _commandDispatcher,
                    instanceConfig,
                    _menuRoot,
                    _globalEvents, 
                    textSurface);

                _registeredSurfaces.Add(textSurface, controller);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"Display Orchestrator failed to add LCD '{textSurface.DisplayName}' to the system.\r\n\r\nMessage: {e.Message}\r\n\r\nStack trace: {e.StackTrace}");
                controller?.Dispose();
            }
        }

        public void UnregisterTextSurface(IMyTextSurface textSurface)
        {
            DisplayController controller;
            if (_registeredSurfaces.TryGetValue(textSurface, out controller))
            {
                controller.Dispose();
                _registeredSurfaces.Remove(textSurface);
            }
        }

        public void RegisterMenuItem(IMenuItem item)
        {
            _menuRoot.AddChild(item);
        }

        private string NextControllerName()
        {
            return $"{_controllerNameTemplate}{++_controllerCounter}";
        }
    }
}