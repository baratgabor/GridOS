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
        /// Implements model travelsal logic, maintains corresponding state, and shapes current model view into a flat string. 
        /// </summary>
        class DisplayViewModel
        {
            // Root is the topmost accessible group (can be any group node); this is our Model
            private IDisplayGroup _rootDisplayGroup;
            private IDisplayGroup _activeDisplayGroup;
            private bool _showBackCommand = false;
            public List<IDisplayElement> ScreenContent { get; private set; } = new List<IDisplayElement>();
            public List<IDisplayGroup> NavigationPath { get; private set; } = new List<IDisplayGroup>();
            public event Action<List<IDisplayElement>> ContentChanged;
            public event Action<List<IDisplayGroup>> PathChanged;

            // Navigation route of user, to support backwards traversal
            private Stack<IDisplayGroup> _navigationStack = new Stack<IDisplayGroup>();
            // Built-in command for handling backwards traversal in tree
            private DisplayCommand _backCommand;

            public DisplayViewModel(IDisplayGroup displayRoot)
            {
                _rootDisplayGroup = displayRoot;
                OpenDisplayGoup(_rootDisplayGroup);
                _backCommand = new DisplayCommand("Back «", MoveBackCommand);
            }

            // TODO: Is this book-keeping excessive? Seems too much to do for a simple context change
            private void OpenDisplayGoup(IDisplayGroup displayGroup)
            {
                if (displayGroup == _rootDisplayGroup)
                    _showBackCommand = false;
                else
                    _showBackCommand = true;

                if (_activeDisplayGroup != null)
                {
                    _activeDisplayGroup.ChildrenChanged -= ChildrenChangedHandler;
                    _activeDisplayGroup.Close();
                }

                _activeDisplayGroup = displayGroup;
                _navigationStack.Push(_activeDisplayGroup);
                _activeDisplayGroup.Open();
                _activeDisplayGroup.ChildrenChanged += ChildrenChangedHandler;
            }

            public void Update()
            {
                UpdateNavigationPath();
                UpdateScreenContent();
            }

            private void UpdateScreenContent()
            {
                ScreenContent.Clear();

                if (_showBackCommand)
                    ScreenContent.Add(_backCommand);

                ScreenContent.AddRange(_activeDisplayGroup.GetChildren());

                ContentChanged?.Invoke(ScreenContent);
            }

            private void UpdateNavigationPath()
            {
                NavigationPath.Clear();
                var navpath = _navigationStack.ToList();
                navpath.Reverse();
                NavigationPath = navpath;

                PathChanged?.Invoke(NavigationPath);
            }

            private void ChildrenChangedHandler(IDisplayGroup displayGroup)
            {
                // TODO: Adjust selection index if needed, e.g. if change was child removal, and selection index might be out of bounds
                UpdateScreenContent();
            }

            public void Execute(IDisplayElement element)
            {
                if (element == null)
                    return;

                if (element is IDisplayGroup)
                {
                    OpenDisplayGoup(element as IDisplayGroup);
                    UpdateNavigationPath();
                    UpdateScreenContent();
                }
                else if (element is IDisplayCommand)
                    (element as IDisplayCommand).Execute();
            }

            private void MoveBackCommand()
            {
                IDisplayElement PreviousGroup = _activeDisplayGroup;

                _activeDisplayGroup.ChildrenChanged -= ChildrenChangedHandler;
                _activeDisplayGroup.Close();
                _navigationStack.Pop();
                _activeDisplayGroup = _navigationStack.Peek();

                if (_activeDisplayGroup == _rootDisplayGroup)
                    _showBackCommand = false;
                else
                    _showBackCommand = true;

                _activeDisplayGroup.Open();
                _activeDisplayGroup.ChildrenChanged += ChildrenChangedHandler;

                UpdateNavigationPath();
                UpdateScreenContent();
            }
        }
    }
}
