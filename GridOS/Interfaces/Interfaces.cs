﻿using Sandbox.Game.EntityComponents;
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

        // TODO: Move these interfaces into more appropriate locations, preferably close to the implementations

        interface IView
        {
            DisplayView AddControl(IControl control);
            void RemoveControl(IControl control);
            void ClearControls();
            void Redraw(StringBuilder content);
        }


        interface IModule
        {
            string ModuleDisplayName { get; }
        }

        interface IUpdateSubscriber
        {
            ObservableUpdateFrequency Frequency { get; }
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
            void DisableUpdates();
            void EnableUpdates();
        }

        interface ICommandDispatcher
        {
            ICommandDispatcher AddCommand(CommandItem command);
            void AddCommands(List<CommandItem> commands);
            void AddCommands_OverwriteExisting(List<CommandItem> commands);
            ICommandDispatcher AddCommand_OverwriteExisting(CommandItem command);
            void RemoveCommands(List<CommandItem> commands);
            void TryDispatch(string commandName);
        }

        public interface IDisplayElement
        {
            string Label { get; set; }
            event Action<IDisplayElement> LabelChanged;
        }

        interface IDisplayCommand : IDisplayElement
        {
            event Action<IDisplayCommand> Executed;
            event Action<IDisplayCommand> BeforeExecute;
            void Execute();
        }

        interface IDisplayGroup : IDisplayElement
        {
            void AddChild(IDisplayElement element);
            void RemoveChild(IDisplayElement element);
            List<IDisplayElement> GetChildren();
            event Action<IDisplayGroup> ChildrenChanged;
            event Action<IDisplayElement> ChildLabelChanged;

            void Open();
            event Action<IDisplayGroup> BeforeOpen;
            event Action<IDisplayGroup> Opened;
           
            void Close();
            event Action<IDisplayGroup> BeforeClose;
            event Action<IDisplayGroup> Closed;

            int OpenedBy { get; }
            bool ShowBackCommandAtBottom { get; }
        }

        interface IDisplayElementPublisher
        {
            IDisplayElement DisplayElement { get; }
        }
    }
}