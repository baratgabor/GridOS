using System.Collections.Generic;
using System;

namespace IngameScript
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

        public event Action<IMenuGroup> ChildrenChanged;
        public event Action<IMenuItem> ChildLabelChanged;
        public event Action<IMenuGroup> Opened;
        public event Action<IMenuGroup> Closed;

        public MenuGroup(string label) : base(label)
        { }

        public void AddChild(IMenuItem item)
        {
            if (_children.Contains(item))
                return;

            _children.Add(item);
            item.LabelChanged += HandleChildrenLabelChanges;
            ChildrenChanged?.Invoke(this);
        }

        public void RemoveChild(IMenuItem item)
        {
            if (!_children.Contains(item))
                return;

            _children.Remove(item);
            item.LabelChanged -= HandleChildrenLabelChanges;
            ChildrenChanged?.Invoke(this);
        }

        public void Open(object context)
        {
            _openedBy++;

            // Invoke only if opened first (multidisplay support)
            if (_openedBy == 1)
                Opened?.Invoke(this);
        }

        public void Close(object context)
        {
            if (_openedBy == 0)
                throw new Exception("Group is not open.");

            _openedBy--;

            // Invoke only if closed by all (multidisplay support)
            if (_openedBy <= 0)
                Closed?.Invoke(this);
        }

        public IEnumerable<IMenuItem> GetChildren()
        {
            return _children;
        }

        protected void HandleChildrenLabelChanges(IMenuItem item)
        {
            // TODO: Consider if this limitation is needed. The class doesn't communicate this fact towards consumers, and it complicates notification logic.
            //if (_openedBy <= 0)
            //    return;

            ChildLabelChanged?.Invoke(item);
        }
    }
}
