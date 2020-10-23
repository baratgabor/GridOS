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
        protected IEnumerable<string> _lastPath;

        public Breadcrumb(IBreadcrumbConfig config)
        {
            _padding = new String(config.PaddingChar, config.PaddingLeft);
            _config = config;
        }

        public StringBuilder GetContent(bool FlushCache = false)
        {
            if (FlushCache)
                BuildContent(_lastPath);

            return _buffer;
        }

        public void OnPathChanged(IEnumerable<string> path)
        {
            BuildContent(path);
            RedrawRequired?.Invoke(this);
        }

        protected void BuildContent(IEnumerable<string> path)
        {
            _lastPath = path;
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
        }
    }
}
