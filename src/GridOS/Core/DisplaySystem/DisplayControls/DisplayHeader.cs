using System.Text;
using System;

namespace IngameScript
{
    class DisplayHeader : Control
    {
        protected readonly StringBuilder _buffer = new StringBuilder();
        protected readonly ProgressIndicator2 _spinner = new ProgressIndicator2();
        protected readonly IDiagnosticService _diagnostics;

        public DisplayHeader(IDiagnosticService diagnostics)
        {
            _diagnostics = diagnostics;

            FontSize = 0.6f;
            WidthUnit = SizeUnit.Percent;
            Width = 100;
            PaddingUnit = SizeUnit.Em;
            Padding = new Thickness(1, 0.2f, 0, 0.2f);

            TextColor = Color.Black;
            BackgroundColor = new Color(255, 255, 255, 90);
        }

        public override StringBuilder GetContent(bool FlushCache = false)
        {
            _buffer.Clear();
            _buffer.Append("GridOS   ");
            _buffer.Append(_spinner.Get());
            return _buffer;
        }
    }
}
