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
        /// Implements model traversal logic, maintains model view context/state, and communicates model updates via event invocations
        /// </summary>
        class MenuHandler : IMenuContentSource, INavigationPathSource
        {
            public IEnumerable<IMenuItem> Content => _content;
            public IEnumerable<IMenuGroup> NavigationPath => _navigationPath;

            protected List<IMenuItem> _content = new List<IMenuItem>();
            protected List<IMenuGroup> _navigationPath = new List<IMenuGroup>();

            public event Action<IEnumerable<IMenuItem>> ContentChanged;
            public event Action<IEnumerable<IMenuGroup>> NavigationPathChanged;

            // Root group is the topmost group that contains all other elements and groups of the tree
            protected IMenuGroup _rootMenuGroup;
            // Active group is the currently open group of the tree
            protected IMenuGroup _activeMenuGroup;

            // Navigation route of user, to support backwards traversal
            protected Stack<IMenuGroup> _navigationStack = new Stack<IMenuGroup>();

            // Built-in command for handling backwards traversal in tree
            protected MenuCommand _backCommand;
            protected MenuCommand _backCommandBottom; // Separate instance; top and bottom back command shouldn't evaluate to equal

            public MenuHandler(IMenuGroup menuRoot)
            {
                _rootMenuGroup = menuRoot;
                _backCommand = new MenuCommand("Back «", MoveBack);
                _backCommandBottom = new MenuCommand("Back «", MoveBack);
                ChangeContextTo(_rootMenuGroup, notifySubscribers: false);
            }

            /// <summary>
            /// Opens another group as the current context. Closes previously opened group (if any was opened).
            /// </summary>
            protected void ChangeContextTo(IMenuGroup menuGroup, bool notifySubscribers = true, bool addToPath = true)
            {
                if (_activeMenuGroup != null)
                    CloseMenuGroup(_activeMenuGroup);

                _activeMenuGroup = menuGroup;

                if (addToPath)
                    _navigationStack.Push(menuGroup);

                OpenMenuGroup(menuGroup);

                if (!notifySubscribers)
                    return;

                UpdateContent();
                UpdateNavigationPath();
            }

            /// <summary>
            /// Closes the current group, and opens the previously opened group (i.e. parent group).
            /// </summary>
            protected void MoveBack()
            {
                // Discard current group from stack
                _navigationStack.Pop();
                // Change context to previous group on stack
                ChangeContextTo(_navigationStack.Peek(), addToPath: false); 
            }

            protected void OpenMenuGroup(IMenuGroup menuGroup)
            {
                menuGroup.Open();
                menuGroup.ChildAdded += Handle_ChildAdded;
                menuGroup.ChildChanged += Handle_ChildContentChanged;
            }

            protected void CloseMenuGroup(IMenuGroup menuGroup)
            {
                menuGroup.ChildAdded -= Handle_ChildAdded;
                menuGroup.ChildChanged -= Handle_ChildContentChanged;
                menuGroup.Close();
            }

            protected void UpdateContent()
            {
                _content.Clear();

                // If active group is not topmost, inject a command for moving one level higher
                if (_activeMenuGroup != _rootMenuGroup)
                    _content.Add(_backCommand);

                _content.AddRange(_activeMenuGroup.GetChildren());

                // If specified, inject another command for moving one level higher at the bottom too
                if (_activeMenuGroup.ShowBackCommandAtBottom)
                    _content.Add(_backCommandBottom);

                ContentChanged?.Invoke(_content);
            }

            protected void UpdateNavigationPath()
            {
                _navigationPath.Clear();
                _navigationPath.AddRange(_navigationStack);
                _navigationStack.Reverse();

                NavigationPathChanged?.Invoke(_navigationPath);
            }

            protected void Handle_ChildAdded(IMenuItem menuItem)
                => UpdateContent();

            // TODO: Specific support for 'ChildLabelChanged' removed, since the planned Caching decorator ...
            // ... should handle fine-grained updates. Examine if further optimization is needed.
            protected void Handle_ChildContentChanged(IMenuItem menuItem)
                => UpdateContent();

            public void Execute(IMenuItem element)
            {
                if (element == null)
                    return;

                // Element is a group, open it
                if (element is IMenuGroup)
                    ChangeContextTo(element as IMenuGroup);

                // Element is a command, execute it
                else if (element is IMenuCommand)
                    (element as IMenuCommand).Execute();
            }
        }
    }
}
