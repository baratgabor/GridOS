using System.Collections.Generic;
using System;

namespace IngameScript
{
    /// <summary>
    /// Modular multitasking and command handling system that can register and run multiple code modules.
    /// Individual modules can contain components (via implementing the appropriate interface) for publishing commands and/or subscribing to recurring automatic execusion.
    /// </summary>
    class GridOS
    {
        private readonly MyGridProgram _p;
        private readonly GlobalEventDispatcher _executionEvents;
        private readonly IDiagnosticServiceController _diagnostics;

        private readonly IUpdateDispatcher _updateDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly DisplayOrchestrator _displayOrchestrator;

        private const string _systemName = "GridOS Experimental";

        // Main module storage
        private readonly List<IModule> _moduleList = new List<IModule>();

        public GridOS(MyGridProgram p)
        {
            _p = p;
            _executionEvents = new GlobalEventDispatcher();
            _diagnostics = new GridProgramDiagnostics(_p) { LoggingLevel = LogLevel.Debug };

            Func<UpdateFrequency> _updateFrequencyGetter = () => _p.Runtime.UpdateFrequency;
            Action<UpdateFrequency> _updateFrequencySetter = (x) => _p.Runtime.UpdateFrequency = x;

            _commandDispatcher = new CommandDispatcher();
            _displayOrchestrator = new DisplayOrchestrator(_commandDispatcher, _diagnostics, _executionEvents);
            _updateDispatcher = new FastUpdateDispatcher((ILogger)_diagnostics, _updateFrequencyGetter, _updateFrequencySetter);

            // TODO: Remove 'help' menu. The only reason it's still here is that it's ideal for testing word wrapping and scrolling.
            _displayOrchestrator.RegisterMenuItem(new HelpMenu());
            _displayOrchestrator.RegisterMenuItem(new SettingsMenu(_diagnostics));

            _commandDispatcher.AddCommand(new CommandItem("AddLcd", CommandHandler_AddLcd));
            _commandDispatcher.AddCommand(new CommandItem("DisableUpdates", CommandHandler_DisableUpdates));
            _commandDispatcher.AddCommand(new CommandItem("EnableUpdates", CommandHandler_EnableUpdates));

            TryExecuteCustomData(_p.Me.CustomData);
        }

        /// <summary>
        /// Registers a module.
        /// </summary>
        /// <param name="module"></param>
        /// <returns>Returns true on success. Returns false if same module was already registed.</returns>
        public bool RegisterModule(IModule module)
        {
            if (_moduleList.Contains(module))
                return false;

            _moduleList.Add(module);

            if (module is IUpdateSubscriber)
            {
                _updateDispatcher.Add(module as IUpdateSubscriber);
            }

            if (module is ICommandPublisher)
            {
                _commandDispatcher.AddCommands((module as ICommandPublisher).Commands);
            }

            if (module is IMenuContentPublisher)
            {
                foreach (var e in ((IMenuContentPublisher)module).MenuItems)
                {
                    _displayOrchestrator.RegisterMenuItem(e);
                }
            }

            return true;
        }

        /// <summary>
        /// This method should receive control from the script's Main() method.
        /// </summary>
        /// <param name="updateType">The UpdateType received in the script's Main() method.</param>
        /// <param name="argument">The argument received in the script's Main() method.</param>
        public void Main(string argument, UpdateType updateType)
        {
            _diagnostics.RecordExecution(logExecutionStats: true);
            _executionEvents.ExecutionStarted();

            // Dispatch updates.
            if (updateType >= UpdateType.Update1)
            {
                try
                {
                    _updateDispatcher.Dispatch(updateType);
                }
                catch (Exception e)
                {
                    _diagnostics.Log(LogLevel.Error, $"Error dispatching update type '{updateType}'. Message: {e.Message}");
                }
            }
                
            // Dispatch commands.
            if (argument != "")
            {
                try
                {
                    _commandDispatcher.TryDispatch(argument.Trim());
                }
                catch (Exception e)
                {
                    _diagnostics.Log(LogLevel.Error, $"Error processing argument '{argument}'. Message: {e.Message}");
                }
            }

            _executionEvents.ExecutionFinishing();
        }

        /// <summary>
        /// Entry point for saving. Call it from Save().
        /// </summary>
        public void Save()
        {
            // TODO: Implement saving and exiting functionality
        }

        public void RegisterTextSurface(IMyTextSurface textSurface)
        {
            _displayOrchestrator.RegisterTextSurface(textSurface);
        }

        private void TryExecuteCustomData(string customData)
        {
            if (customData == string.Empty)
                return;

            foreach (var line in customData.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                _commandDispatcher.TryDispatch(line.Trim());
            }
        }

        // TODO: GridOS class shouldn't contain low-level logic, move this somewhere else.
        private void CommandHandler_AddLcd(CommandItem sender, string param)
        {
            int surfaceIndex = 0;

            {   // Extract optional surface index
                int delimiter = param.IndexOf(' ');
                if (delimiter > -1)
                {
                    if (int.TryParse(
                        param.Substring(delimiter + 1, param.Length - (delimiter + 1)), out surfaceIndex))
                    {
                        surfaceIndex = Math.Max(0, 
                            int.Parse(param.Substring(delimiter + 1, param.Length - (delimiter + 1)))
                            - 1);
                    }

                    param = param.Substring(0, delimiter);
                }
            }

            IMyTerminalBlock target = _p.GridTerminalSystem.GetBlockWithName(param);
            
            if (target == null)
                return;

            if (target is IMyTextSurface)
                RegisterTextSurface((IMyTextSurface)target);

            if (target is IMyTextSurfaceProvider)
                RegisterTextSurface(
                    ((IMyTextSurfaceProvider)target).GetSurface(surfaceIndex));
        }

        private void CommandHandler_DisableUpdates(CommandItem sender, string param)
        {
            _updateDispatcher.DisableUpdates();
        }

        private void CommandHandler_EnableUpdates(CommandItem sender, string param)
        {
            _updateDispatcher.EnableUpdates();
        }
    }
}