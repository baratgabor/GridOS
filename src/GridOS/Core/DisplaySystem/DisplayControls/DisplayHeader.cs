using System.Text;
using System;

namespace IngameScript
{
    class DisplayHeader : Control
    {
        protected readonly StringBuilder _buffer = new StringBuilder();
        protected readonly ProgressIndicator2 _spinner = new ProgressIndicator2();
        protected readonly string _displayId;

        public DisplayHeader(string displayId)
        {
            _displayId = displayId;
            FontSize = 0.6f;
            WidthUnit = SizeUnit.Percent;
            Width = 100;
            PaddingUnit = SizeUnit.Em;
            Padding = new Thickness(1, 0.2f, 1, 0.2f);
            TextColor = Color.Black;
            BackgroundColor = new Color(255, 255, 255, 90);
        }

        public override void Dispose()
        {}

        public override StringBuilder GetContent(ContentGenerationHelper _, bool FlushCache = false)
        {
            _buffer.Clear();
            _buffer.Append("GridOS – ");
            _buffer.Append(_displayId);
            _buffer.Append(' ');
            _buffer.Append(_spinner.Get());
            return _buffer;
        }
    }
}
