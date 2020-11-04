using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    /// <summary>
    /// Creates a formatted string representation of position inside a hierarchical tree.
    /// Should receive updated path data through <see cref="OnPathChanged(List{string})"/>.
    /// </summary>
    class Breadcrumb : Control
    {
        protected StringBuilder _buffer = new StringBuilder();
        protected IBreadcrumbConfig _config;
        protected IEnumerable<string> _lastPath;

        public Breadcrumb(IBreadcrumbConfig config)
        {
            _config = config;

            TextColor = new Color(255, 255, 235, 120);
            BackgroundColor = new Color(0, 0, 0, 80);
            FontSize = 0.7f;
            PaddingUnit = SizeUnit.Em;
            Padding = new Thickness(0.7f, 0.5f, 0, 0.5f);
            Width = 100;
            WidthUnit = SizeUnit.Percent;
        }

        public override void Dispose()
        {}

        public override StringBuilder GetContent(ContentGenerationHelper _, bool FlushCache = false)
        {
            if (FlushCache)
                BuildContent(_lastPath);

            return _buffer;
        }

        public void OnPathChanged(IEnumerable<string> path)
        {
            BuildContent(path);
            OnRedrawRequired();
        }

        protected void BuildContent(IEnumerable<string> path)
        {
            _lastPath = path;
            _buffer.Clear();

            foreach (var name in path)
            {
                _buffer.Append(name);
                _buffer.Append(' ');
                _buffer.Append(_config.PathSeparator);
                _buffer.Append(' ');
            }

            _buffer.Length -= 3; // Trim trailing path separator.
        }
    }
}
