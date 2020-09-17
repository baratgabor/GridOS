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
        /// Specialized node type that holds children nodes, used for creating hierarchical structures.
        /// </summary>
        class MenuGroup : MenuItem, IMenuGroup
        {
            public int OpenedBy => _openedBy;
            protected int _openedBy = 0;

            public bool ShowBackCommandAtBottom { get; internal set; } = false;

            protected List<IMenuItem> _children = new List<IMenuItem>();

            public event Action<IMenuItem> ChildAdded;
            public event Action<IMenuItem> ChildRemoved;
            public event Action<IMenuItem> ChildChanged;
            public event Action<IMenuGroup> Opening;
            public event Action<IMenuGroup> Opened;
            public event Action<IMenuGroup> Closing;
            public event Action<IMenuGroup> Closed;

            public MenuGroup(string label) : base(label)
            { }

            public void AddChild(IMenuItem element)
            {
                if (_children.Contains(element))
                    return;

                _children.Add(element);
                element.LabelChanged += HandleChildrenLabelChanged;
                ChildAdded?.Invoke(element);
            }

            public void RemoveChild(IMenuItem element)
            {
                if (!_children.Contains(element))
                    return;

                _children.Remove(element);
                element.LabelChanged -= HandleChildrenLabelChanged;
                ChildRemoved?.Invoke(element);
            }

            public void Open()
            {
                _openedBy++;

                // Invoke only if opened first (multidisplay support)
                if (_openedBy != 1) return;

                Opening?.Invoke(this);

                // Do here what?

                Opened?.Invoke(this);
            }

            public void Close()
            {
                _openedBy--;

                // Invoke only if closed by all (multidisplay support)
                if (_openedBy != 0) return;

                Closing?.Invoke(this);

                // Do here what?

                Closed?.Invoke(this);
            }

            public List<IMenuItem> GetChildren()
            {
                return _children;
            }

            protected void HandleChildrenLabelChanged(IMenuItem element)
            {
                if (_openedBy <= 0)
                    return;

                ChildChanged?.Invoke(this);
            }
        }
    }
}
