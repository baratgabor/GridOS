using System;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Simple, non-actionable element to be displayed, with support for update propagation.
        /// </summary>
        public interface IDisplayElement
        {
            /// <summary>
            /// The content to be displayed.
            /// </summary>
            string Label { get; set; }

            /// <summary>
            /// Event that notifies subscribers when the content changes.
            /// </summary>
            event Action<IDisplayElement> LabelChanged;
        }
    }
}
