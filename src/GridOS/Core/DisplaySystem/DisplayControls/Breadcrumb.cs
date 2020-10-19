using System.Collections.Generic;
using System.Text;
using System;

namespace IngameScript
{
    /// <summary>
    /// Creates a formatted string representation of position inside a hierarchical tree.
    /// Should receive updated path data through <see cref="OnPathChanged(List{string})"/>.
    /// </summary>
    class Breadcrumb : IControl
    {
        public event Action<IControl> RedrawRequired;

        protected StringBuilder _buffer = new StringBuilder();
        protected string _padding = String.Empty;
        protected IBreadcrumbConfig _config;

        public Breadcrumb(IBreadcrumbConfig config)
        {
            // TODO: consider whether this needs to be updated when config changes
            _padding = new String(config.PaddingChar, config.PaddingLeft);
            _config = config;
        }

        public StringBuilder GetContent(bool FlushCache = false)
        {
            // Need to ignore flush request here, since we don't have any means of pulling new data
            return _buffer;
        }

        public void OnPathChanged(IEnumerable<string> path)
        { 
            _buffer.Clear();

            _buffer.Append(_config.SeparatorLineTop);
            _buffer.AppendLine();

            _buffer.Append(_padding);
                
            foreach (var name in path)
            {
                _buffer.Append(name);
                _buffer.Append(' ');
                _buffer.Append(_config.PathSeparator);
                _buffer.Append(' ');
            }

            _buffer.Length -= 3; // Trim trailing path separator.

            _buffer.AppendLine();
            _buffer.Append(_config.SeparatorLineBottom);

            // TODO: Implement path string shortening if it exceeds a certain length (i.e. line length)
            RedrawRequired?.Invoke(this);
        }
    }
}
