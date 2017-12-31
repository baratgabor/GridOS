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
        interface IModule
        {
            string ModuleDisplayName { get; }
        }

        interface IUpdateSubscriber
        {
            ObservableUpdateFrequency UpdateFrequency { get; }
            void Update(UpdateType updateType);
        }

        interface ICommandPublisher
        {
            List<CommandItem> Commands { get; }
        }

        interface IUpdateDispatcherAndController
        {
            void Add(IUpdateSubscriber updateSubscriber);
            void Remove(IUpdateSubscriber updateSubscriber);
            void Dispatch(UpdateType UpdateType);
        }

        interface ICommandDispatcher
        {
            void AddCommands(List<CommandItem> commands);
            void RemoveCommands(List<CommandItem> commands);
            bool TryDispatch(string commandName);
        }

        interface ICommandMenu
        { }

        interface ISystemDisplay
        { }
    }
}