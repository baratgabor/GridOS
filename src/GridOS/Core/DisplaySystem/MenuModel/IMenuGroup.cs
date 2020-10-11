using System.Collections.Generic;
using System;

namespace IngameScript
{
    /// <summary>
    /// Extends the basic menu item with composite behavior, i.e. the addition and removal of other items as children.
    /// Used for grouping items together to display contextually related aggregates.
    /// </summary>
    interface IMenuGroup : IMenuItem
    {
        /// <summary>
        /// Adds a child under this group.
        /// </summary>
        void AddChild(IMenuItem item);

        /// <summary>
        /// Removes a child from this group.
        /// </summary>
        void RemoveChild(IMenuItem item);

        /// <summary>
        /// Returns the list of children.
        /// </summary>
        IEnumerable<IMenuItem> GetChildren();

        /// <summary>
        /// Notifies when the list of children are altered.
        /// </summary>
        event Action<IMenuGroup> ChildrenChanged;

        /// <summary>
        /// Notifies when the content of a child is changed.
        /// </summary>
        event Action<IMenuItem> ChildLabelChanged;

        /// <summary>
        /// Marks this group as opened. This allows us to notify any components that are interested to know when the group is opened.
        /// </summary>
        void Open();

        /// <summary>
        /// Notifies when the group is opened.
        /// </summary>
        event Action<IMenuGroup> Opened;

        /// <summary>
        /// Marks this group as closed. This allows us to notify any components that are interested to know when the group is closed.
        /// </summary>           
        void Close();

        /// <summary>
        /// Notifies when the group is closed.
        /// </summary>
        event Action<IMenuGroup> Closed;

        /// <summary>
        /// Indicates how many times this group is open on various displays.
        /// </summary>
        int OpenedBy { get; }

        /// <summary>
        /// Indicates that this group should contain a back navigation command as the last item.
        /// </summary>
        // TODO: This violates SRP. Group should be content-agnostic. Find another way to mark groups for bottom back nav.
        bool ShowBackCommandAtBottom { get; }
    }
}
