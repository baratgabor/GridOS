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

        static class Configuration
        {
            public static readonly string Setting1 = "value";
            public static readonly string Setting2 = "value";
        }

        public string Bla = "1";

        class ShipOS
        {
            private Action<string> _echo;
            private IMyGridProgramRuntimeInfo _runtime;

            private IUpdateDispatcherAndController _updateDispatcherAndController;
            // TODO: Need to make sure that registered Commands are unique, unless we want to allow executing multiple Modules with the same command name
            private ICommandDispatcher _commandDispatcher;
            private ShipSystemDisplay _shipSystemDisplay;

            private const string _systemName = "ShipOS 0.8 Experimental";
            private double _totalExecTime = 0;
            private int _numOfExec = 0;
            private int _lastInstrCount = 0;
            private bool _initialPeriodExceeded = false;
            
            // Main module storage
            private List<IModule> _moduleList = new List<IModule>();

            public ShipOS(Action<string> echo, IMyGridProgramRuntimeInfo runtime, IUpdateDispatcherAndController updateDispatcherAndController, ICommandDispatcher commandDispatcher, ShipSystemDisplay shipSystemDisplay)
            {
                _echo = echo;
                _runtime = runtime;
                _updateDispatcherAndController = updateDispatcherAndController;
                _commandDispatcher = commandDispatcher;
                _shipSystemDisplay = shipSystemDisplay;
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
                    // TODO: here too, we maintain the list of commands in two places :/ better solution needed.
                    _commandDispatcher.AddCommands((module as ICommandPublisher).Commands);
                    _shipSystemDisplay.AddCommands((module as ICommandPublisher).Commands);
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
                    ArgumentInterpreter(argument);
                }

                _lastInstrCount = _runtime.CurrentInstructionCount;
            }

            private void ArgumentInterpreter(string argument)
            {
                _echo("ArgumentInterpreter invoked.");

                int numericalParam = 0;
                if (Int32.TryParse(argument, out numericalParam))
                {
                    _echo("ProcessDisplayCommand called.");
                    _shipSystemDisplay.ProcessDisplayCommand(numericalParam);
                }
                else
                {
                    _echo("CommandDispatcher.TryDispatch called.");
                    _commandDispatcher.TryDispatch(argument.Trim());

                    /*
                    string[] stringParams = argument.Trim().Split(' ');
                    for (int x = 0; x == stringParams.Length; x++)
                    {
                        _commandDispatcher.TryDispatch(stringParams[x]);
                    }
                    */
                }
            }

            public void RegisterTextPanel(IMyTextPanel textPanel)
            {
                _shipSystemDisplay.RegisterTextPanel(textPanel);
            }
        }
    }
}