using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    public interface IWordWrapper
    {
        IEnumerable<StringSegment> WordWrap(string input, float maxWidthCorrection);
        float MeasureStringWidthInPixels(string input);
    }

    /// <summary>
    /// Wraps lines to fill a given text surface.
    /// </summary>
    // TODO: Replace this naive implementation with an optimized one once you're not nauseated by the though of writing word wrappers.
    class TextSurfaceWordWrapper : IWordWrapper
    {
        private readonly IDisplayConfig _config;
        private readonly IMyTextSurface _surface;
        private readonly StringBuilder _buffer = new StringBuilder();
        private readonly char[] _wordDelimiters = new[] { ' ', '-' };

        public TextSurfaceWordWrapper(IDisplayConfig config)
        {
            _config = config;
            _surface = config.OutputSurface;
        }

        public float MeasureStringWidthInPixels(string input)
        {
            return _surface.MeasureStringInPixels(_buffer.Clear().Append(input), _config.FontName, _config.FontSize).X;
        }

        public IEnumerable<StringSegment> WordWrap(string input, float maxWidthCorrection)
        {
            _buffer.Clear();
            var fontSize = _config.FontSize;
            var fontName = _config.FontName;
            var maxWidth = _config.OutputWidth;

            int lastBreakableIndex = 0;
            int lastLineBreakIndex = 0;
            bool yieldLastBreakable = false;

            while (true)
            {
                int nextBreakableIndex = input.IndexOfAny(_wordDelimiters, lastBreakableIndex);

                if (nextBreakableIndex == -1)
                {
                    lastBreakableIndex = input.Length;
                    yieldLastBreakable = true;
                }
                else
                {
                    _buffer.Append(input, lastBreakableIndex, nextBreakableIndex - lastBreakableIndex + 1);
                    var lineWidth = _surface.MeasureStringInPixels(_buffer, fontName, fontSize).X;

                    if (lineWidth > maxWidth + maxWidthCorrection)
                    {
                        yieldLastBreakable = true;
                    }
                }

                if (yieldLastBreakable)
                {
                    var newLineIndex = input.IndexOf(Environment.NewLine, lastLineBreakIndex, lastBreakableIndex - lastLineBreakIndex);
                    if (newLineIndex > -1)
                    {
                        yield return new StringSegment(input, lastLineBreakIndex, newLineIndex - lastLineBreakIndex);
                        lastLineBreakIndex = newLineIndex + Environment.NewLine.Length;
                        nextBreakableIndex = newLineIndex + Environment.NewLine.Length - 1;
                    }
                    else
                    {
                        yield return new StringSegment(input, lastLineBreakIndex, lastBreakableIndex - lastLineBreakIndex);
                        lastLineBreakIndex = lastBreakableIndex;
                    }

                    _buffer.Clear();
                    if (lastLineBreakIndex == input.Length)
                        break;

                    yieldLastBreakable = false;
                }

                lastBreakableIndex = nextBreakableIndex + 1;
            }
        }
    }
}
