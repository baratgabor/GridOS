using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Modular multitasking and command handling system that can register and run multiple code modules.
        /// Individual modules can contain components (via implementing the appropriate interface) for publishing commands and/or subscribing to recurring automatic execusion.
        /// </summary>
        class GridOS
        {
            private MyGridProgram _p;
            private Action<string> _echo;
            private IMyGridProgramRuntimeInfo _runtime;

            private IUpdateDispatcher _updateDispatcher;
            private ICommandDispatcher _commandDispatcher;
            private DisplayOrchestrator _displayOrchestrator;

            private const string _systemName = "GridOS Experimental";
            private double _totalExecTime = 0;
            private int _numOfExec = 0;
            private int _lastInstrCount = 0;
            private bool _initialPeriodExceeded = false;

            // Main module storage
            private List<IModule> _moduleList = new List<IModule>();

            public GridOS(MyGridProgram p)
            {
                _p = p;
                _echo = _p.Echo;
                _runtime = _p.Runtime;

                Func<UpdateFrequency> _updateFrequencyGetter = () => _runtime.UpdateFrequency;
                Action<UpdateFrequency> _updateFrequencySetter = (x) => _runtime.UpdateFrequency = x;

                _commandDispatcher = new CommandDispatcher();
                _updateDispatcher = new UpdateDispatcher_v1(_echo, _updateFrequencyGetter, _updateFrequencySetter);
                _displayOrchestrator = new DisplayOrchestrator(_commandDispatcher, p);

                _commandDispatcher.AddCommand(new CommandItem("AddLcd", CommandHandler_AddLcd));
                _commandDispatcher.AddCommand(new CommandItem("DisableUpdates", CommandHandler_DisableUpdates));
                _commandDispatcher.AddCommand(new CommandItem("EnableUpdates", CommandHandler_EnableUpdates));
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

                if (module is IDisplayElementPublisher)
                {
                    foreach (var e in ((IDisplayElementPublisher)module).DisplayElements)
                    {
                        _displayOrchestrator.RegisterDisplayElement(e);
                    }
                }

                return true;
            }

            /// <summary>
            /// Entry point for each update cycle. Call it from Main().
            /// </summary>
            /// <param name="updateType">The UpdateType received in Main().</param>
            /// <param name="argument">The argument received in Main().</param>
            public void Main(string argument, UpdateType updateType)
            {
                // TODO: Get rid of this ad-hoc diagnostics, move it somewhere sane
                _totalExecTime += _runtime.LastRunTimeMs;
                _numOfExec++;
                double _avgExecTime = _totalExecTime / _numOfExec;

                if (_numOfExec == 10 && _initialPeriodExceeded == false)
                {
                    _totalExecTime = 0;
                    _numOfExec = 0;
                    _initialPeriodExceeded = true;
                }

                //_echo("ExecuteCycle invoked.");
                _echo("Last Instr. Count: " + _lastInstrCount);
                _echo($"Last Execusion Time: {_runtime.LastRunTimeMs:G3}");
                _echo($"Average Execusion Time: {_avgExecTime:G3}");

                // TODO: Structure exception handling properly
                _updateDispatcher.Dispatch(updateType);
                try
                {
                    if (argument != "")
                    {
                        _commandDispatcher.TryDispatch(argument.Trim());
                    }
                }
                catch(Exception e)
                {
                    _echo(e.ToString());
                }

                _lastInstrCount = _runtime.CurrentInstructionCount;
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

            private void CommandHandler_AddLcd(CommandItem sender, string param)
            {
                IMyTerminalBlock textSurface = _p.GridTerminalSystem.GetBlockWithName(param);
                if ((textSurface == null) || !(textSurface is IMyTextSurface))
                    return;

                RegisterTextSurface(textSurface as IMyTextSurface);
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
}