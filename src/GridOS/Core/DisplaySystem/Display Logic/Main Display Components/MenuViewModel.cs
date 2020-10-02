using System.Collections.Generic;
using System;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Implements model travelsal logic, maintains model view context/state, and communicates model updates via event invocations
        /// </summary>
        class MenuViewModel
        {
            // Root is the topmost accessible group (can be any group node); this is our Model
            private IMenuGroup _rootMenuGroup;
            private IMenuGroup _activeMenuGroup;
            private bool _showBackCommand = false;
            public List<IMenuItem> Content { get; private set; } = new List<IMenuItem>();
            public List<IMenuGroup> NavigationPath { get; private set; } = new List<IMenuGroup>();
            public event Action<List<IMenuItem>> ContentChanged; // Items added or removed
            public event Action<IMenuItem> ItemChanged; // Single item label changed
            public event Action<ContentChangeInfo> PathChanged; // When moving to another group

            // Navigation route of user, to support backwards traversal
            private Stack<IMenuGroup> _navigationStack = new Stack<IMenuGroup>();
            private List<string> _path = new List<string>();

            // Built-in command for handling backwards traversal in tree
            private MenuCommand _backCommand;
            private MenuCommand _backCommandBottom; // Separate instance; top and bottom back command shouldn't evaluate to equal

            public MenuViewModel(IMenuGroup menuRoot)
            {
                _rootMenuGroup = menuRoot;
                ChangeContextTo(_rootMenuGroup);
                _backCommand = new MenuCommand("Back «", MoveBackCommand);
                _backCommandBottom = new MenuCommand("Back «", MoveBackCommand);
            }

            private void ChangeContextTo(IMenuGroup menuGroup)
            {
                if (_activeMenuGroup != null)
                    CloseMenuGroup(_activeMenuGroup);

                _activeMenuGroup = menuGroup;
                _navigationStack.Push(menuGroup);
                OpenMenuGroup(menuGroup);
            }

            private void CloseMenuGroup(IMenuGroup menuGroup)
            {
                menuGroup.ChildrenChanged -= Handle_ChildrenChanged;
                menuGroup.ChildLabelChanged -= Handle_ChildLabelChanged;
                menuGroup.Close();
            }

            private void OpenMenuGroup(IMenuGroup menuGroup)
            {
                if (menuGroup == _rootMenuGroup)
                    _showBackCommand = false;
                else
                    _showBackCommand = true;

                menuGroup.Open();
                menuGroup.ChildrenChanged += Handle_ChildrenChanged;
                menuGroup.ChildLabelChanged += Handle_ChildLabelChanged;
            }

            public void PushUpdate(IMenuGroup previousContext = null)
            {
                PathChanged?.Invoke(new ContentChangeInfo(
                    content: UpdateContent(),
                    navigationPath: UpdateNavigationPath(),
                    previousContext: previousContext
                    ));
            }

            private List<IMenuItem> UpdateContent()
            {
                Content.Clear();

                if (_showBackCommand)
                    Content.Add(_backCommand);

                Content.AddRange(_activeMenuGroup.GetChildren());

                if (_activeMenuGroup.ShowBackCommandAtBottom)
                    Content.Add(_backCommandBottom);

                return Content;
            }

            private List<string> UpdateNavigationPath()
            {
                _path.Clear();

                foreach (var e in _navigationStack)
                    _path.Add(e.Label);

                _path.Reverse();

                return _path;
            }

            private void Handle_ChildrenChanged(IMenuGroup menuGroup)
            {
                ContentChanged?.Invoke(UpdateContent());
            }

            private void Handle_ChildLabelChanged(IMenuItem item)
            {
                ItemChanged?.Invoke(item);
            }

            public void Execute(IMenuItem item)
            {
                if (item == null)
                    return;

                if (item is IMenuGroup)
                {
                    ChangeContextTo(item as IMenuGroup);
                    PushUpdate();
                }
                else if (item is IMenuCommand)
                    (item as IMenuCommand).Execute();
            }

            private void MoveBackCommand()
            {
                IMenuGroup previousGroup = _activeMenuGroup;

                CloseMenuGroup(_activeMenuGroup);
                _navigationStack.Pop();
                _activeMenuGroup = _navigationStack.Peek();
                OpenMenuGroup(_activeMenuGroup);

                PushUpdate(previousGroup);
            }
        }
    }
}
