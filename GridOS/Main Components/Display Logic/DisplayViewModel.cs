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
            private int _selectedIndex;
            private int _currentLowBound => _showBackCommand ? -1 : 0;
            private StringBuilder _builder = new StringBuilder();
            public List<string> ScreenContent { get; private set; } = new List<string>();
            public List<string> NavigationPath { get; private set; } = new List<string>();
            public int CursorPosition => _showBackCommand ? _selectedIndex + 1 : _selectedIndex;
            public event Action<List<string>> ContentChanged;
            public event Action<List<string>> PathChanged;
            public event Action<int> SelectionChanged;

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

                _selectedIndex = _currentLowBound;
            }

            public void Update()
            {
                UpdateNavigationPathString();
                UpdateScreenContentString();
            }

            private void UpdateScreenContentString()
            {
                var groupContent = _activeDisplayGroup.GetChildren();

                ScreenContent.Clear();

                if (_showBackCommand)
                    ScreenContent.Add("Back «");

                foreach (var e in groupContent)
                    ScreenContent.Add((e is IDisplayGroup) ? e.Label + " »" : e.Label);

                ContentChanged?.Invoke(ScreenContent);
            }

            private void UpdateNavigationPathString()
            {
                NavigationPath.Clear();
                var pathList = _navigationStack.ToList();
                pathList.Reverse();
                foreach (var p in pathList)
                    NavigationPath.Add(p.Label);

                PathChanged?.Invoke(NavigationPath);
                SelectionChanged?.Invoke(CursorPosition);
            }

            private void ChildrenChangedHandler(IDisplayGroup displayGroup)
            {
                // TODO: Adjust selection index if needed, e.g. if change was child removal, and selection index might be out of bounds
                UpdateScreenContentString();
            }

            public void MoveUp(CommandItem sender, string param)
            {
                if (_selectedIndex <= _currentLowBound)
                    return;

                _selectedIndex--;

                // Only cursor position changes, no need to rebuild string list here
                SelectionChanged?.Invoke(CursorPosition);
            }

            public void MoveDown(CommandItem sender, string param)
            {
                if (_selectedIndex >= _activeDisplayGroup.GetChildren().Count - 1)
                    return;

                _selectedIndex++;

                // Only cursor position changes, no need to rebuild string list here
                SelectionChanged?.Invoke(CursorPosition);
            }

            public void Select(CommandItem sender, string param)
            {
                IDisplayElement selectedElement;
                if ((_showBackCommand) && (_selectedIndex == -1))
                    selectedElement = _backCommand;
                else
                    selectedElement = _activeDisplayGroup.GetChildren().ElementAtOrDefault(_selectedIndex);

                if (selectedElement == null)
                    return;

                if (selectedElement is IDisplayGroup)
                {
                    OpenDisplayGoup(selectedElement as IDisplayGroup);
                    UpdateNavigationPathString();
                    UpdateScreenContentString();
                }
                else if (selectedElement is IDisplayCommand)
                    (selectedElement as IDisplayCommand).Execute();
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

                // TODO: Don't assume that group content is the same; check to make sure
                _selectedIndex = _activeDisplayGroup.GetChildren().IndexOf(PreviousGroup);

                UpdateNavigationPathString();
                UpdateScreenContentString();
            }
        }
    }
}
