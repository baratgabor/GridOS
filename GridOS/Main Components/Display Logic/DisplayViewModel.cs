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
            private int? _selectedIndex;
            private string _contentString;
            private StringBuilder _builder = new StringBuilder();

            public string Content => _contentString;
            public int? CursorPosition => _selectedIndex;
            public event Action<string, int?> ContentChanged;

            // Navigation route of user, to support backwards traversal
            private Stack<IDisplayGroup> _navigationStack = new Stack<IDisplayGroup>();
            // Built-in command for handling backwards traversal in tree
            private DisplayCommand _backCommand;

            public DisplayViewModel(IDisplayGroup displayRoot)
            {
                _rootDisplayGroup = displayRoot;
                OpenDisplayGoup(_rootDisplayGroup);
                UpdateStringRepresentation();

                _backCommand = new DisplayCommand("Back <<", MoveBackCommand);
            }

            // TODO: Is this book-keeping excessive? Seems too much to do for a simple context change
            private void OpenDisplayGoup(IDisplayGroup displayGroup)
            {
                if (_activeDisplayGroup != null)
                {
                    _activeDisplayGroup.ChildrenChanged -= ChildrenChangedHandler;
                    _activeDisplayGroup.Close();
                }

                _activeDisplayGroup = displayGroup;

                // TODO: Get rid of this hack; implement a different way of dealing with the "Back" command
                if ((_activeDisplayGroup != _rootDisplayGroup) && (!_activeDisplayGroup.GetChildren().Contains(_backCommand)))
                    _activeDisplayGroup.GetChildren().Insert(0, _backCommand);

                _navigationStack.Push(_activeDisplayGroup);
                _activeDisplayGroup.Open();
                _activeDisplayGroup.ChildrenChanged += ChildrenChangedHandler;

                _selectedIndex = 0;
            }

            public void UpdateStringRepresentation()
            {
                var groupContent = _activeDisplayGroup.GetChildren();
                if (groupContent.Count == 0)
                    return;

                var selectedElement = groupContent.ElementAtOrDefault((int)_selectedIndex);
                if (selectedElement == null)
                    return;

                // TODO: Don't hard-code the marker. Also don't hardcode the prefixing whitespaces below. These are presentation details; need to be injected at least.
                string marker = ">";
                _builder.Clear();

                foreach (var e in groupContent)
                {
                    if (e == selectedElement)
                        _builder.Append($" {marker} ");
                    else
                        _builder.Append($"    ");

                    _builder.Append(e.Label);

                    if (e is IDisplayGroup)
                        _builder.Append(" >>");

                    _builder.AppendLine();
                }

                _contentString = _builder.ToString();
                ContentChanged?.Invoke(_contentString, CursorPosition);
            }

            private void ChildrenChangedHandler(IDisplayGroup displayGroup)
            {
                UpdateStringRepresentation();
            }

            public void MoveUp()
            {
                if (_selectedIndex <= 0)
                    return;

                _selectedIndex--;

                UpdateStringRepresentation();
            }

            public void MoveDown()
            {
                if (_selectedIndex >= _activeDisplayGroup.GetChildren().Count - 1)
                    return;

                _selectedIndex++;

                UpdateStringRepresentation();
            }

            public void Select()
            {
                var selectedElement = _activeDisplayGroup.GetChildren().ElementAtOrDefault((int)_selectedIndex);
                if (selectedElement == null)
                    return;

                if (selectedElement is IDisplayGroup)
                {
                    OpenDisplayGoup(selectedElement as IDisplayGroup);
                    UpdateStringRepresentation();
                }

                if (selectedElement is IDisplayCommand)
                    (selectedElement as IDisplayCommand).Execute();
            }

            private void MoveBackCommand()
            {
                IDisplayElement PreviousGroup = _activeDisplayGroup;

                _activeDisplayGroup.ChildrenChanged -= ChildrenChangedHandler;
                _activeDisplayGroup.Close();
                _navigationStack.Pop();
                _activeDisplayGroup = _navigationStack.Peek();
                _activeDisplayGroup.Open();
                _activeDisplayGroup.ChildrenChanged += ChildrenChangedHandler;

                // TODO: Don't assume that group content is the same; check to make sure
                _selectedIndex = _activeDisplayGroup.GetChildren().IndexOf(PreviousGroup);

                UpdateStringRepresentation();
            }
        }
    }
}
