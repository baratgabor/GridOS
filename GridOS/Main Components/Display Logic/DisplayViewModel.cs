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
            private IDisplayElement _selectedElement {
                get
                {
                    if ((_showBackCommand) && (_selectedIndex == -1))
                        return _backCommand;
                    else
                        return _activeDisplayGroup.GetChildren().ElementAtOrDefault(_selectedIndex);
                }
            }
            private int _currentLowBound => _showBackCommand ? -1 : 0;
            private StringBuilder _builder = new StringBuilder();
            public List<string> Content { get; private set; } = new List<string>();
            public List<string> Path { get; private set; } = new List<string>();
            public int CursorPosition
            {
                get
                {
                    if (_showBackCommand)
                        return _selectedIndex + 1;
                    else
                        return _selectedIndex;
                }
            }
            public event Action<List<string>, int> ContentChanged;
            public event Action<List<string>> PathChanged;

            // Navigation route of user, to support backwards traversal
            private Stack<IDisplayGroup> _navigationStack = new Stack<IDisplayGroup>();
            // Built-in command for handling backwards traversal in tree
            private DisplayCommand _backCommand;

            public DisplayViewModel(IDisplayGroup displayRoot)
            {
                _rootDisplayGroup = displayRoot;
                OpenDisplayGoup(_rootDisplayGroup);
                //UpdatePathString();
                //UpdateStringRepresentation();
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

            public void UpdateStringRepresentation()
            {
                var groupContent = _activeDisplayGroup.GetChildren();

                Content.Clear();

                if (_showBackCommand)
                    Content.Add("Back «");

                int i = 0;
                foreach (var e in groupContent)
                {
                    Content.Add((e is IDisplayGroup) ? e.Label + " »" : e.Label);

                    i++;
                }

                ContentChanged?.Invoke(Content, CursorPosition);
            }

            public void UpdatePathString()
            {
                Path.Clear();
                var pathList = _navigationStack.ToList();
                pathList.Reverse();
                foreach (var p in pathList)
                    Path.Add(p.Label);

                PathChanged?.Invoke(Path);
            }

            private void ChildrenChangedHandler(IDisplayGroup displayGroup)
            {
                // TODO: Adjust selection index if needed, e.g. if change was child removal, and selection index might be out of bounds
                UpdateStringRepresentation();
            }

            public void MoveUp(CommandItem sender, string param)
            {
                if (_selectedIndex <= _currentLowBound)
                    return;

                _selectedIndex--;

                UpdateStringRepresentation();
            }

            public void MoveDown(CommandItem sender, string param)
            {
                if (_selectedIndex >= _activeDisplayGroup.GetChildren().Count - 1)
                    return;

                _selectedIndex++;

                UpdateStringRepresentation();
            }

            public void Select(CommandItem sender, string param)
            {
                if (_selectedElement == null)
                    return;

                if (_selectedElement is IDisplayGroup)
                {
                    OpenDisplayGoup(_selectedElement as IDisplayGroup);
                    UpdatePathString();
                    UpdateStringRepresentation();
                }
                else if (_selectedElement is IDisplayCommand)
                    (_selectedElement as IDisplayCommand).Execute();
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

                UpdatePathString();
                UpdateStringRepresentation();
            }
        }
    }
}
