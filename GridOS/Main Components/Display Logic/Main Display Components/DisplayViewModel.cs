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
        // TODO: Move this struct to somewhere proper
        struct ContentChangeInfo
        {
            public readonly List<IDisplayElement> Content;
            public readonly List<string> NavigationPath;
            public readonly IDisplayGroup PreviousContext;

            public ContentChangeInfo(List<IDisplayElement> content, List<string> navigationPath, IDisplayGroup previousContext)
            {
                Content = content;
                NavigationPath = navigationPath;
                PreviousContext = previousContext;
            }
        }

        /// <summary>
        /// Implements model travelsal logic, maintains corresponding state, and shapes current model view into a flat string. 
        /// </summary>
        class DisplayViewModel
        {
            // Root is the topmost accessible group (can be any group node); this is our Model
            private IDisplayGroup _rootDisplayGroup;
            private IDisplayGroup _activeDisplayGroup;
            private bool _showBackCommand = false;
            public List<IDisplayElement> Content { get; private set; } = new List<IDisplayElement>();
            public List<IDisplayGroup> NavigationPath { get; private set; } = new List<IDisplayGroup>();
            public event Action<List<IDisplayElement>> ContentChanged; // Elements added or removed
            public event Action<IDisplayElement> ElementChanged; // Single element label changed
            public event Action<ContentChangeInfo> PathChanged; // When moving to another group

            // Navigation route of user, to support backwards traversal
            private Stack<IDisplayGroup> _navigationStack = new Stack<IDisplayGroup>();
            private List<string> _path = new List<string>();

            // Built-in command for handling backwards traversal in tree
            private DisplayCommand _backCommand;
            private DisplayCommand _backCommandBottom; // Separate instance; top and bottom back command shouldn't evaluate to equal

            public DisplayViewModel(IDisplayGroup displayRoot)
            {
                _rootDisplayGroup = displayRoot;
                ChangeContextTo(_rootDisplayGroup);
                _backCommand = new DisplayCommand("Back «", MoveBackCommand);
                _backCommandBottom = new DisplayCommand("Back «", MoveBackCommand);
            }

            private void ChangeContextTo(IDisplayGroup displayGroup)
            {
                if (_activeDisplayGroup != null)
                    CloseDisplayGroup(_activeDisplayGroup);

                _activeDisplayGroup = displayGroup;
                _navigationStack.Push(displayGroup);
                OpenDisplayGroup(displayGroup);
            }

            private void CloseDisplayGroup(IDisplayGroup displayGroup)
            {
                displayGroup.ChildrenChanged -= Handle_ChildrenChanged;
                displayGroup.ChildLabelChanged -= Handle_ChildLabelChanged;
                displayGroup.Close();
            }

            private void OpenDisplayGroup(IDisplayGroup displayGroup)
            {
                if (displayGroup == _rootDisplayGroup)
                    _showBackCommand = false;
                else
                    _showBackCommand = true;

                displayGroup.Open();
                displayGroup.ChildrenChanged += Handle_ChildrenChanged;
                displayGroup.ChildLabelChanged += Handle_ChildLabelChanged;
            }

            public void PushUpdate(IDisplayGroup previousContext = null)
            {
                PathChanged?.Invoke(new ContentChangeInfo(
                    content: UpdateContent(),
                    navigationPath: UpdateNavigationPath(),
                    previousContext: previousContext
                    ));
            }

            private List<IDisplayElement> UpdateContent()
            {
                Content.Clear();

                if (_showBackCommand)
                    Content.Add(_backCommand);

                Content.AddRange(_activeDisplayGroup.GetChildren());

                if (_activeDisplayGroup.ShowBackCommandAtBottom)
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

            private void Handle_ChildrenChanged(IDisplayGroup displayGroup)
            {
                ContentChanged?.Invoke(UpdateContent());
            }

            private void Handle_ChildLabelChanged(IDisplayElement element)
            {
                ElementChanged?.Invoke(element);
            }

            public void Execute(IDisplayElement element)
            {
                if (element == null)
                    return;

                if (element is IDisplayGroup)
                {
                    ChangeContextTo(element as IDisplayGroup);
                    PushUpdate();
                }
                else if (element is IDisplayCommand)
                    (element as IDisplayCommand).Execute();
            }

            private void MoveBackCommand()
            {
                IDisplayGroup previousGroup = _activeDisplayGroup;

                CloseDisplayGroup(_activeDisplayGroup);
                _navigationStack.Pop();
                _activeDisplayGroup = _navigationStack.Peek();
                OpenDisplayGroup(_activeDisplayGroup);

                PushUpdate(previousGroup);
            }
        }
    }
}
