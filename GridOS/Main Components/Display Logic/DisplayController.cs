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
            public string Name => _name;
            private string _name;
            private DisplayView _view;
            private DisplayViewModel _viewModel;
            private List<CommandItem> _navCommands;
            private ICommandDispatcher _commandDispatcher;

            public DisplayController(string name, ICommandDispatcher commandDispatcher, DisplayView view, DisplayViewModel viewModel)
            {
                _name = name;
                _commandDispatcher = commandDispatcher;

                _view = view;
                _viewModel = viewModel;
                _viewModel.ContentChanged += _view.Handle_ContentChanged;
                _viewModel.UpdateStringRepresentation();

                _navCommands = new List<CommandItem>()
                {
                    new CommandItem($"{_name}Up", _viewModel.MoveUp),
                    new CommandItem($"{_name}Down", _viewModel.MoveDown),
                    new CommandItem($"{_name}Select", _viewModel.Select)
                };

                _commandDispatcher.AddCommands_OverwriteExisting(_navCommands);
            }

            // TODO: Support command configurationc changes in some way?
            public void UpdateCommandConfiguration()
            {
                throw new Exception("Not implemented");
            }
        }
    }
}
