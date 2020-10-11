using System.Collections.Generic;
using System;

namespace IngameScript
{
    /// <summary>
    /// Basic node in composite pattern based tree structure, used for displaying non-interactive text content.
    /// Serves as a base class for other node types.
    /// </summary>
    public class MenuItem : IMenuItem
    {
        public string Label
        {
            get { return _label; }
            set { SetAndNotify(ref _label, value); }
        }
        protected string _label;

        public event Action<IMenuItem> LabelChanged;

        public MenuItem(string label)
        {
            _label = label;
        }

        // TODO: Notification can be simplified, no need for generics; it seems we won't have more properties.
        protected void SetAndNotify<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            LabelChanged?.Invoke(this);
            return;
        }
    }
}
