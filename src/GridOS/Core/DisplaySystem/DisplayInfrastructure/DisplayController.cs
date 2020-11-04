using System;

namespace IngameScript
{
    /// <summary>
    /// Knows how to instantiate and wire together the full stack of classes needed for a working view system.
    /// Sets up and mediates the execution of display related commands.
    /// </summary>
    class DisplayController : IDisposable, IMenuInstanceServices
    {
        public string Name { get; }
        public IDisplayConfig DisplayConfig { get; }
        public IMenuPresentationConfig MenuConfig { get; }

        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IView _view;
        private readonly IGlobalEvents _globalEvents;
        private readonly Breadcrumb _breadcrumb;
        private readonly Menu _menu;

        private readonly CommandItem _upCommand;
        private readonly CommandItem _downCommand;
        private readonly CommandItem _selectCommand;

        public DisplayController(string name, ICommandDispatcher commandDispatcher, BaseConfig config, IMenuGroup menuRoot, IGlobalEvents globalEvents, IMyTextSurface surface)
        {
            Name = name;
            MenuConfig = config;
            DisplayConfig = config;
            _globalEvents = globalEvents;
            _commandDispatcher = commandDispatcher;

            try
            {
                _view = new DisplayView(
                    surface,
                    config,
                    new TextSurfaceWordWrapper()
                );

                _globalEvents.ExecutionWillFinish += DisplayTick;

                _upCommand = new CommandItem($"{Name}Up", OnMoveUp);
                _downCommand = new CommandItem($"{Name}Down", OnMoveDown);
                _selectCommand = new CommandItem($"{Name}Select", OnSelect);

                _menu = new Menu(
                    new MenuModel(menuRoot, this),
                    config);
                _breadcrumb = new Breadcrumb(config);
                _menu.NavigationPathChanged += _breadcrumb.OnPathChanged;

                _view
                    .AddControl(new DisplayHeader())
                    .AddControl(_breadcrumb)
                    .AddControl(_menu);

                _commandDispatcher
                    .AddCommand_OverwriteExisting(_upCommand)
                    .AddCommand_OverwriteExisting(_downCommand)
                    .AddCommand_OverwriteExisting(_selectCommand);

                _menu.PushUpdate();
            }
            catch (Exception)
            {
                _view?.Dispose();
                _view = null;
                _menu?.Dispose();
                _menu = null;
                throw;
            }
        }

        public void SetFontType(string fontName)
            => _view.SetFontType(fontName);

        public void SetFontSize(float fontSize)
            => _view.SetFontSize(fontSize);

        public void SetFontColor(Color color)
            => _view.SetFontColor(color);

        public void SetBackgroundColor(Color color)
            => _view.SetBackgroundColor(color);

        public void Dispose()
        {
            _globalEvents.ExecutionWillFinish -= DisplayTick;
            _menu.NavigationPathChanged -= _breadcrumb.OnPathChanged;
            _commandDispatcher.RemoveCommand(_upCommand);
            _commandDispatcher.RemoveCommand(_downCommand);
            _commandDispatcher.RemoveCommand(_selectCommand);
            _view.Dispose();
        }

        private void OnMoveUp(CommandItem _, string __)
            => _menu.MoveUp();

        private void OnMoveDown(CommandItem _, string __)
            => _menu.MoveDown();

        private void OnSelect(CommandItem _, string __)
            => _menu.Select();

        private void DisplayTick()
            => _view.Redraw();
    }
}
