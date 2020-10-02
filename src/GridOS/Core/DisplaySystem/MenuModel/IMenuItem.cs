using System;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Simple, non-actionable item to be displayed, with support for update propagation.
        /// </summary>
        public interface IMenuItem
        {
            /// <summary>
            /// The content to be displayed.
            /// </summary>
            string Label { get; set; }

            /// <summary>
            /// Event that notifies subscribers when the content changes.
            /// </summary>
            event Action<IMenuItem> LabelChanged;
        }
    }
}
