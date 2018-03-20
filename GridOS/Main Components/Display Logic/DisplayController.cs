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
            private IMyGridProgramRuntimeInfo _runtime;

            private SmartConfig _config = new SmartConfig();

            private NavigationFrame _navigation;
            private MenuContentBuilder _menuBuilder;
            private Breadcrumb _breadcrumb;

            public DisplayController(string name, ICommandDispatcher commandDispatcher, DisplayView view, DisplayViewModel viewModel, IMyGridProgramRuntimeInfo runtime)
            {
                _name = name;
                _commandDispatcher = commandDispatcher;
                _runtime = runtime;

                _view = view;
                _viewModel = viewModel;

                _breadcrumb = new Breadcrumb(_config);

                _menuBuilder = new MenuContentBuilder(_config);
                _menuBuilder
                    .AddProcessor(new WordWrap_BreakPresearchStrategy(_config))
                    .AddProcessor(new AddPrefix())
                    .AddProcessor(new AddSuffix())
                    .AddProcessor(new PadAllLines(_config))
                    .AddProcessor(new LineInfoExtractor(_config));

                _view
                    .AddControl(new DisplayHeader(_config, _runtime))
                    .AddControl(_breadcrumb)
                    .AddControl(_menuBuilder);

                _navigation = new NavigationFrame(_config,
                    new ScrollableFrame(_config,
                        _menuBuilder));

                _viewModel.PathChanged += _breadcrumb.OnPathChanged;
                _viewModel.ContentChanged += _menuBuilder.OnContentChanged;
                _viewModel.ElementChanged += _menuBuilder.OnElementChanged;
                _navigation.ItemSelected += _viewModel.Execute;
                
                _commandDispatcher
                    .AddCommand_OverwriteExisting(new CommandItem($"{_name}Up", _navigation.MoveUp))
                    .AddCommand_OverwriteExisting(new CommandItem($"{_name}Down", _navigation.MoveDown))
                    .AddCommand_OverwriteExisting(new CommandItem($"{_name}Select", _navigation.Select));

                _viewModel.PushUpdate();
            }
        }
    }
}
