using System.Collections.Generic;
using System;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Extends the basic display element with composite behavior, i.e. the addition and removal of other elements as children.
        /// Used for grouping elements together to display contextually related aggregates.
        /// </summary>
        interface IDisplayGroup : IDisplayElement
        {
            /// <summary>
            /// Adds a child under this group element.
            /// </summary>
            void AddChild(IDisplayElement element);

            /// <summary>
            /// Removes a child from this group element.
            /// </summary>
            void RemoveChild(IDisplayElement element);

            /// <summary>
            /// Returns the list of children.
            /// </summary>
            IEnumerable<IDisplayElement> GetChildren();

            /// <summary>
            /// Notifies when the list of children are altered.
            /// </summary>
            event Action<IDisplayGroup> ChildrenChanged;

            /// <summary>
            /// Notifies when the display content of a child is changed.
            /// </summary>
            event Action<IDisplayElement> ChildLabelChanged;

            /// <summary>
            /// Marks this group as opened. This allows us to notify any components that are interested to know when the group is opened.
            /// </summary>
            void Open();

            /// <summary>
            /// Notifies when the group is opened.
            /// </summary>
            event Action<IDisplayGroup> Opened;

            /// <summary>
            /// Marks this group as closed. This allows us to notify any components that are interested to know when the group is closed.
            /// </summary>           
            void Close();

            /// <summary>
            /// Notifies when the group is closed.
            /// </summary>
            event Action<IDisplayGroup> Closed;

            /// <summary>
            /// Indicates how many times this group is open on various displays.
            /// </summary>
            int OpenedBy { get; }

            /// <summary>
            /// Indicates that this group should contain a back navigation command as the last element.
            /// </summary>
            // TODO: This violates SRP. Group should be content-agnostic. Find another way to mark groups for bottom back nav.
            bool ShowBackCommandAtBottom { get; }
        }
    }
}
