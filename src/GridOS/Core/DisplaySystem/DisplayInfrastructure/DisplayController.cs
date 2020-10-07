using System;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Knows how to instantiate and wire together the full stack of classes needed for a working view system.
        /// Sets up and mediates the execution of display related commands.
        /// </summary>
        class DisplayController : IDisposable
        {
            public string Name { get; }

            private readonly ICommandDispatcher _commandDispatcher;
            private readonly MyGridProgram _program;
            private readonly IView _view;
            private readonly Breadcrumb _breadcrumb;
            private readonly Menu _menu;

            private readonly CommandItem _upCommand;
            private readonly CommandItem _downCommand;
            private readonly CommandItem _selectCommand;

            public DisplayController(string name, ICommandDispatcher commandDispatcher, SmartConfig config, MyGridProgram program, IView view, IMenuGroup menuRoot)
            {
                Name = name;
                _view = view;
                _program = program;
                _commandDispatcher = commandDispatcher;

                _upCommand = new CommandItem($"{Name}Up", OnMoveUp);
                _downCommand = new CommandItem($"{Name}Down", OnMoveDown);
                _selectCommand = new CommandItem($"{Name}Select", OnSelect);

                _menu = new Menu(
                    new MenuModel(menuRoot),
                    config);
                _breadcrumb = new Breadcrumb(config);
                _menu.NavigationPathChanged += _breadcrumb.OnPathChanged;

                _view
                    .AddControl(new DisplayHeader(config, _program.Runtime))
                    .AddControl(_breadcrumb)
                    .AddControl(_menu);

                _commandDispatcher
                    .AddCommand_OverwriteExisting(_upCommand)
                    .AddCommand_OverwriteExisting(_downCommand)
                    .AddCommand_OverwriteExisting(_selectCommand);

                _menu.PushUpdate();
            }

            public void Dispose()
            {
                _commandDispatcher.RemoveCommand(_upCommand);
                _commandDispatcher.RemoveCommand(_downCommand);
                _commandDispatcher.RemoveCommand(_selectCommand);
            }

            private void OnMoveUp(CommandItem _, string __)
                => _menu.MoveUp();

            private void OnMoveDown(CommandItem _, string __)
                => _menu.MoveDown();

            private void OnSelect(CommandItem _, string __)
                => _menu.Select();
        }
    }
}
