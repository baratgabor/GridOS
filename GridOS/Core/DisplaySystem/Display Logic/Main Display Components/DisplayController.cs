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
        /// Knows how to instantiate and wire together the full stack of classes needed for a working view system.
        /// Doesn't receive inputs directly, but sets up static routing between command dispatcher and nagivation handler.
        /// </summary>
        class DisplayController
        {
            public string Name => _name;
            private string _name;
            private IView _view;
            private MenuViewModel _viewModel;
            private ICommandDispatcher _commandDispatcher;
            private MyGridProgram _program;

            private NavigationFrame _navigation;
            private MenuContentBuilder _menuBuilder;
            private Breadcrumb _breadcrumb;

            public DisplayController(string name, ICommandDispatcher commandDispatcher, SmartConfig config, IView view, MenuViewModel viewModel, MyGridProgram program)
            {
                _name = name;
                _program = program;
                _commandDispatcher = commandDispatcher;

                _view = view;
                _viewModel = viewModel;

                _breadcrumb = new Breadcrumb(config);

                _menuBuilder = new MenuContentBuilder(config);
                _menuBuilder.ContentSource = () => _viewModel.Content;
                _menuBuilder
                    .AddProcessor(new WordWrap_BreakPresearchStrategy(config))
                    .AddProcessor(new AddPrefix())
                    .AddProcessor(new AddSuffix())
                    .AddProcessor(new PadAllLines(config))
                    .AddProcessor(new LineInfoExtractor(config));
                
                _navigation = new NavigationFrame(config,
                    new ScrollableFrame(config,
                        _menuBuilder));

                _view
                    .AddControl(new DisplayHeader(config, _program.Runtime))
                    .AddControl(_breadcrumb)
                    .AddControl(_navigation);

                // TODO: The order of subscription here matters; refactor it
                _viewModel.PathChanged += _breadcrumb.OnPathChanged;
                _viewModel.PathChanged += _navigation.OnPathChanged;
                //_viewModel.PathChanged += _menuBuilder.OnPathChanged; // _navigation handles all now
                _viewModel.ContentChanged += _menuBuilder.OnContentChanged;
                _viewModel.ItemChanged += _menuBuilder.OnItemChanged;
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
