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
        /// Bundles a View and a ViewModel instance, and sets up their interaction.
        /// Receives commands and routes them to methods exposed by ViewModel.
        /// </summary>
        class DisplayController
        {
            private DisplayView _view;
            private DisplayViewModel _viewModel;
            private Dictionary<NavigationCommand, Action> _commandMap = new Dictionary<Program.NavigationCommand, Action>();

            public DisplayController(DisplayView view, DisplayViewModel viewModel)
            {
                _view = view;
                _viewModel = viewModel;
                _viewModel.ContentChanged += _view.Handle_ContentChanged;
                //_viewModel.UpdateStringRepresentation();

                _commandMap.Add(NavigationCommand.Up, _viewModel.MoveUp);
                _commandMap.Add(NavigationCommand.Down, _viewModel.MoveDown);
                _commandMap.Add(NavigationCommand.Select, _viewModel.Select);
            }

            public void ProcessCommand(NavigationCommand command)
            {
                if (_commandMap.ContainsKey(command))
                    _commandMap[command].Invoke();
            }

            // TODO: Support command configurationc changes in some way?
            public void UpdateCommandConfiguration()
            {
                throw new Exception("Not implemented");
            }
        }
    }
}
