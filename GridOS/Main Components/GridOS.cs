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
        /// Modular multitasking and command handling system that can register and run multiple code modules.
        /// Individual modules can contain components (via implementing the appropriate interface) for publishing commands and/or subscribing to recurring automatic execusion.
        /// </summary>
        class GridOS
        {
            // TODO: implement proper logging with a cross cutting concern pattern; same with try...catch on components and modules

            private MyGridProgram _p;
            private Action<string> _echo;
            private IMyGridProgramRuntimeInfo _runtime;

            private IUpdateDispatcherAndController _updateDispatcherAndController;
            // TODO: Need to make sure that registered Commands are unique, unless we want to allow executing multiple Modules with the same command name
            private ICommandDispatcher _commandDispatcher;
            private DisplayOrchestrator _displayOrchestrator;

            private const string _systemName = "ShipOS 0.8 Experimental";
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
                _updateDispatcherAndController = new UpdateDispatcherAndController1(_echo, _updateFrequencyGetter, _updateFrequencySetter);
                _displayOrchestrator = new DisplayOrchestrator(_commandDispatcher);
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
                    // TODO: we maintain the list of modules here and in this class too... think about another solution.
                    _updateDispatcherAndController.Add(module as IUpdateSubscriber);
                }

                if (module is ICommandPublisher)
                {
                    _commandDispatcher.AddCommands((module as ICommandPublisher).Commands);
                }

                if (module is IDisplayElementPublisher)
                {
                    _displayOrchestrator.RegisterDisplayElement((module as IDisplayElementPublisher).DisplayElement);
                }

                return true;
            }

            /// <summary>
            /// The main entry point for each update cycle. Call it from Main().
            /// </summary>
            /// <param name="updateType">The UpdateType received in Main().</param>
            /// <param name="argument">The argument received in Main().</param>
            public void ExecuteCycle(string argument, UpdateType updateType)
            {
                _totalExecTime += _runtime.LastRunTimeMs;
                _numOfExec++;
                double _avgExecTime = _totalExecTime / _numOfExec;

                if (_numOfExec == 10 && _initialPeriodExceeded == false)
                {
                    _totalExecTime = 0;
                    _numOfExec = 0;
                    _initialPeriodExceeded = true;
                }

                _echo("ExecuteCycle invoked.");
                _echo($"Last Instr. Count: {_lastInstrCount}");
                _echo($"Last Execusion Time: {_runtime.LastRunTimeMs:G3}");
                _echo($"Average Execusion Time: {_avgExecTime:G3}");

                _updateDispatcherAndController.Dispatch(updateType);

                if (argument != "")
                {
                    _commandDispatcher.TryDispatch(argument.Trim());
                }

                _lastInstrCount = _runtime.CurrentInstructionCount;
            }

            public void RegisterTextPanel(IMyTextPanel textPanel)
            {
                _displayOrchestrator.RegisterTextPanel(textPanel);
            }
        }
    }
}