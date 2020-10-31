using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    /// <summary>
    /// This menu group miraculously composites singleton menu items with display instance dependent menu items.
    /// In this way it's able to provide settings for each display connected to the system, besides the global settings.
    /// </summary>
    class SettingsMenu : MenuGroup
    {
        protected Dictionary<IMenuInstance, List<IMenuItem>> _contextualChildren = new Dictionary<IMenuInstance, List<IMenuItem>>();

        public SettingsMenu(IDiagnosticServiceController diagnostics) : base("Preferences")
        {           
            AddChild(new LogLevelCommand(diagnostics));
        }

        public override IEnumerable<IMenuItem> GetChildren(IMenuInstance menuInstance = null)
        {
            if (menuInstance == null)
                throw new Exception($"This menu group requires a valid {nameof(menuInstance)} to be specified.");
            else
                return Enumerable.Concat(
                    base.GetChildren(menuInstance),
                    GetContextualChildren(menuInstance));
        }

        private IEnumerable<IMenuItem> GetContextualChildren(IMenuInstance menuInstance = null)
        {
            if (!_contextualChildren.ContainsKey(menuInstance))
            {
                _contextualChildren.Add(menuInstance, CreateContextualList(menuInstance));
            }

            return _contextualChildren[menuInstance];
        }

        private List<IMenuItem> CreateContextualList(IMenuInstance menuInstance)
        {
            var services = menuInstance.MenuInstanceServices;
            return new List<IMenuItem>() {
                new FontSizeCommand(services),
                new FontColorCommand(services),
                new BackgroundColorCommand(services)
            };
        }
    }
}
