﻿using Sandbox.Game.EntityComponents;
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
        class DisplayGroup : DisplayElement, IDisplayGroup
        {
            public int OpenedBy => _openedBy;
            protected int _openedBy = 0;

            protected List<IDisplayElement> _children = new List<IDisplayElement>();

            public event Action<IDisplayGroup> ChildrenChanged;
            public event Action<IDisplayGroup> BeforeOpen;
            public event Action<IDisplayGroup> Opened;
            public event Action<IDisplayGroup> BeforeClose;
            public event Action<IDisplayGroup> Closed;

            public DisplayGroup(string label) : base(label)
            { }

            public void AddChild(IDisplayElement element)
            {
                if (_children.Contains(element))
                    return;

                _children.Add(element);
                element.LabelChanged += HandleChildrenLabelChanges;
                ChildrenChanged?.Invoke(this);
            }

            public void RemoveChild(IDisplayElement element)
            {
                if (!_children.Contains(element))
                    return;

                _children.Remove(element);
                element.LabelChanged -= HandleChildrenLabelChanges;
                ChildrenChanged?.Invoke(this);
            }

            public void Open()
            {
                BeforeOpen?.Invoke(this);
                _openedBy++;
                Opened?.Invoke(this);
            }

            public void Close()
            {
                BeforeClose?.Invoke(this);
                _openedBy--;
                Closed?.Invoke(this);
            }

            public List<IDisplayElement> GetChildren()
            {
                return _children;
            }

            protected void HandleChildrenLabelChanges(IDisplayElement element)
            {
                if (_openedBy <= 0)
                    return;

                ChildrenChanged?.Invoke(this);
            }
        }
    }
}
