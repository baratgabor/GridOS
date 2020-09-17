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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        class NavigationProcessor
        {
            // Move up and Move down wired in here

            // First we check if viewport selection can simply just go up/down by a line
            // if so, simply call viewport selection moveup or movedown

            // if not, then viewport line selection doesn't need to be modified
            // but viewport content needs to be scrolled up or down by a line
            // and then selection marker needs to be added to this newly assembled content

            // OR we need a different mechanism for selection marker addition, instead of treating it like a wrapper/processor on the content

            // BUT the fact might help a lot that we know that when scrolling is needed, selection marker will always be ...
            // at the FIRST line (if scrolling up) ...
            // or the LAST line (if scrolling down) ...
            // so possibly we can simply skip executing the selection component
        }

        class ViewportWithSelection
        {
            public string Content => _content.ToString();

            protected StringBuilder _content = new StringBuilder();

            protected string[] _lines;
            protected SelectionTracker _selectionTracker = new SelectionTracker(0, 1);
            protected readonly int _selectionMarkerPosition;
            protected readonly char _selectionMarker;

            public ViewportWithSelection(int viewportHeight, int selectionMarkerPosition, char selectionMarker)
            {
                _lines = new string[viewportHeight];
                _selectionTracker.MaxIndex = viewportHeight - 1;

                _selectionMarkerPosition = selectionMarkerPosition;
                _selectionMarker = selectionMarker;
            }

            public bool MoveUp() => _selectionTracker.MoveUp();
            public bool MoveDown() => _selectionTracker.MoveDown();
        }


        class SelectionTracker
        {
            public int MaxIndex
            {
                get => _maxIndex;
                set {
                    if (_maxIndex <= _minIndex)
                        throw new InvalidOperationException($"{nameof(MaxIndex)} must be larger than {nameof(MinIndex)}.");
                    _maxIndex = value;
                    EnforceSelectionBounds();
                }
            }

            public int MinIndex
            {
                get => _minIndex;
                set
                {
                    if (_minIndex >= _maxIndex)
                        throw new InvalidOperationException($"{nameof(MinIndex)} must be smaller than {nameof(MaxIndex)}.");
                    _minIndex = value;
                    EnforceSelectionBounds();
                }
            }

            public int SelectedIndex
                => _selectedIndex;

            protected int _selectedIndex = 0;
            protected int _minIndex;
            protected int _maxIndex;

            public SelectionTracker(int minIndex, int maxIndex)
            {
                MinIndex = minIndex;
                MaxIndex = maxIndex;
            }

            public bool MoveDown()
            {
                if (_selectedIndex >= _maxIndex)
                    return false;

                _selectedIndex++;
                return true;
            }

            public bool MoveUp()
            {
                if (_selectedIndex <= _minIndex)
                    return false;

                _selectedIndex--;
                return true;
            }

            protected void EnforceSelectionBounds()
            {
                if (_selectedIndex < _minIndex)
                    _selectedIndex = _minIndex;

                if (_selectedIndex > _maxIndex)
                    _selectedIndex = _maxIndex;
            }
        }

        interface IMenu
        {
            bool MoveUp();
            bool MoveDown();
            bool Select();

            event Action Navigating;
            event Action Navigated;

            IEnumerable<IMenuItem> MenuContent { get; }
            IEnumerable<IMenuGroup> ContentPath { get; }
        }

        interface IMenuElementProcessor
        {
            
        }

        interface IMenuProcessor
        {

        }


        /// <summary>
        /// Aggregate class that functions as a mediator between constituent menu components
        /// </summary>
        class Menu
        {
            public event Action<IMenuItem> ItemAdded;
            public event Action<IMenuItem> ItemRemoved;
            public event Action<IMenuItem> ItemChanged;

            protected MenuNavigator _menuNavigator;
            protected SelectionTracker _menuSelectionTracker;

            public Menu(MenuNavigator menuNavigator, SelectionTracker menuSelectionTracker)
            {
                _menuNavigator = menuNavigator;
                _menuSelectionTracker = menuSelectionTracker;

                _menuNavigator.ItemAdded += OnItemAdded;
                _menuNavigator.ItemRemoved += OnItemRemoved;
                _menuNavigator.ItemChanged += OnItemChanged;
                _menuNavigator.Navigated += OnNavigated;

                SetSelectionBounds();
            }

            protected void OnItemChanged(IMenuItem item)
            {

                ItemChanged?.Invoke(item);
            }

            protected void OnItemRemoved(IMenuItem item)
            {
                // TODO: get index of removed item, and adjust selection of index is lower than selected index
                SetSelectionBounds();
                ItemRemoved?.Invoke(item);
            }

            protected void OnItemAdded(IMenuItem item)
            {
                SetSelectionBounds();
                ItemAdded?.Invoke(item);
            }

            protected void OnNavigated()
            {
                SetSelectionBounds();
            }

            protected void SetSelectionBounds()
            {
                _menuSelectionTracker.MaxIndex = _menuNavigator.Content.Count();
            }

            public bool MoveUp()
                => _menuSelectionTracker.MoveUp();

            public bool MoveDown()
                => _menuSelectionTracker.MoveDown();

            public bool Select()
                => _menuNavigator.Select(_menuSelectionTracker.SelectedIndex);
        }



        /// <summary>
        /// Implements model traversal logic, maintains model view context/state, and communicates model updates via event invocations
        /// </summary>
        class MenuNavigator : IMenuContentSource, INavigationPathSource
        {
            public IEnumerable<IMenuItem> Content => _content;
            public IEnumerable<IMenuGroup> NavigationPath => _navigationPath;

            protected List<IMenuItem> _content = new List<IMenuItem>();
            protected List<IMenuGroup> _navigationPath = new List<IMenuGroup>();

            //public event Action<IEnumerable<IMenuItem>> ContentChanged;
            //public event Action<IEnumerable<IMenuGroup>> NavigationPathChanged;

            public event Action Navigating;
            public event Action Navigated;

            public event Action<IMenuItem> ItemChanged;
            public event Action<IMenuItem> ItemAdded;
            public event Action<IMenuItem> ItemRemoved;
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

            public MenuNavigator(IMenuGroup menuRoot)
            {
                _rootMenuGroup = menuRoot;
                _backCommand = new MenuCommand("Back «", NavigateToParent);
                _backCommandBottom = new MenuCommand("Back «", NavigateToParent);
                NavigateTo(_rootMenuGroup, notifySubscribers: false);
            }

            /// <summary>
            /// Opens another group as the current context. Closes previously opened group (if any was opened).
            /// </summary>
            protected void NavigateTo(IMenuGroup menuGroup, bool notifySubscribers = true, bool addToPath = true)
            {
                if (notifySubscribers)
                    Navigating?.Invoke();

                if (_activeMenuGroup != null)
                    CloseMenuGroup(_activeMenuGroup);

                _activeMenuGroup = menuGroup;

                if (addToPath)
                    _navigationStack.Push(menuGroup);

                OpenMenuGroup(menuGroup);
                UpdateContent();
                UpdateNavigationPath();

                if (notifySubscribers)
                    Navigated?.Invoke();
            }

            /// <summary>
            /// Closes the current group, and opens the previously opened group (i.e. parent group).
            /// </summary>
            protected void NavigateToParent()
            {
                // Discard current group from stack
                _navigationStack.Pop();
                // Navigate to previous group on stack
                NavigateTo(_navigationStack.Peek(), addToPath: false);
            }

            protected void OpenMenuGroup(IMenuGroup menuGroup)
            {
                menuGroup.Open();
                menuGroup.ChildAdded += Handle_ChildAdded;
                menuGroup.ChildRemoved += Handle_ChildRemoved;
                menuGroup.ChildChanged += Handle_ChildLabelChanged;
            }

            protected void CloseMenuGroup(IMenuGroup menuGroup)
            {
                menuGroup.ChildAdded -= Handle_ChildAdded;
                menuGroup.ChildRemoved -= Handle_ChildRemoved;
                menuGroup.ChildChanged -= Handle_ChildLabelChanged;
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
            }

            protected void UpdateNavigationPath()
            {
                _navigationPath.Clear();
                _navigationPath.AddRange(_navigationStack);
                _navigationStack.Reverse();
            }

            protected void Handle_ChildLabelChanged(IMenuItem item)
            {
                UpdateContent();
                ItemChanged?.Invoke(item);
            }

            protected void Handle_ChildRemoved(IMenuItem item)
            {
                UpdateContent();
                ItemRemoved?.Invoke(item);
            }

            protected void Handle_ChildAdded(IMenuItem item)
            {
                UpdateContent();
                ItemAdded?.Invoke(item);
            }

            public bool Select(int itemIndex)
                => Select(_content[itemIndex]);

            protected bool Select(IMenuItem element)
            {
                if (element == null)
                    return false;

                // Element is a group, open it
                if (element is IMenuGroup)
                {
                    NavigateTo(element as IMenuGroup);
                    return true;
                }

                // Element is a command, execute it
                if (element is IMenuCommand)
                {
                    (element as IMenuCommand).Execute();
                    return true;
                }

                return false;
            }
        }
    }
}
