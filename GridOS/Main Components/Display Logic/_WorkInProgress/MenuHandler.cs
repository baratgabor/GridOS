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
            public IEnumerable<IDisplayElement> Content => _content;
            public IEnumerable<IDisplayGroup> NavigationPath => _navigationPath;

            protected List<IDisplayElement> _content = new List<IDisplayElement>();
            protected List<IDisplayGroup> _navigationPath = new List<IDisplayGroup>();

            public event Action<IEnumerable<IDisplayElement>> ContentChanged;
            public event Action<IEnumerable<IDisplayGroup>> NavigationPathChanged;

            // Root group is the topmost group that contains all other elements and groups of the tree
            protected IDisplayGroup _rootDisplayGroup;
            // Active group is the currently open group of the tree
            protected IDisplayGroup _activeDisplayGroup;

            // Navigation route of user, to support backwards traversal
            protected Stack<IDisplayGroup> _navigationStack = new Stack<IDisplayGroup>();

            // Built-in command for handling backwards traversal in tree
            protected DisplayCommand _backCommand;
            protected DisplayCommand _backCommandBottom; // Separate instance; top and bottom back command shouldn't evaluate to equal

            public MenuHandler(IDisplayGroup displayRoot)
            {
                _rootDisplayGroup = displayRoot;
                ChangeContextTo(_rootDisplayGroup, notifySubscribers: false);
                _backCommand = new DisplayCommand("Back «", MoveBack);
                _backCommandBottom = new DisplayCommand("Back «", MoveBack);
            }

            /// <summary>
            /// Opens another group as the current context. Closes previously opened group (if any was opened).
            /// </summary>
            protected void ChangeContextTo(IDisplayGroup displayGroup, bool notifySubscribers = true, bool pushToStack = true)
            {
                if (_activeDisplayGroup != null)
                    CloseDisplayGroup(_activeDisplayGroup);

                _activeDisplayGroup = displayGroup;

                if (pushToStack)
                    _navigationStack.Push(displayGroup);

                OpenDisplayGroup(displayGroup);

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
                ChangeContextTo(_navigationStack.Peek(), pushToStack: false); 
            }

            protected void OpenDisplayGroup(IDisplayGroup displayGroup)
            {
                displayGroup.Open();
                displayGroup.ChildrenChanged += Handle_ChildrenChanged;
                displayGroup.ChildLabelChanged += Handle_ChildLabelChanged;
            }

            protected void CloseDisplayGroup(IDisplayGroup displayGroup)
            {
                displayGroup.ChildrenChanged -= Handle_ChildrenChanged;
                displayGroup.ChildLabelChanged -= Handle_ChildLabelChanged;
                displayGroup.Close();
            }

            protected void UpdateContent()
            {
                _content.Clear();

                // If active group is not topmost, inject a command for moving one level higher
                if (_activeDisplayGroup != _rootDisplayGroup)
                    _content.Add(_backCommand);

                _content.AddRange(_activeDisplayGroup.GetChildren());

                // If specified, inject another command for moving one level higher at the bottom too
                if (_activeDisplayGroup.ShowBackCommandAtBottom)
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

            protected void Handle_ChildrenChanged(IDisplayGroup displayGroup)
                => UpdateContent();

            // TODO: Specific support for 'ChildLabelChanged' removed, since the planned Caching decorator ...
            // ... should handle fine-grained updates. Examine if further optimization is needed.
            protected void Handle_ChildLabelChanged(IDisplayElement displayElement)
                => UpdateContent();

            public void Execute(IDisplayElement element)
            {
                if (element == null)
                    return;

                // Element is a group, open it
                if (element is IDisplayGroup)
                    ChangeContextTo(element as IDisplayGroup);

                // Element is a command, execute it
                else if (element is IDisplayCommand)
                    (element as IDisplayCommand).Execute();
            }
        }
    }
}
