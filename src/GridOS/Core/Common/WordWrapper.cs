using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IngameScript
{
    public interface IWordWrapperController : IWordWrapper
    {
        IWordWrapperController SetUp(float maxLineLength, IMyTextSurface surface, string fontName, float fontSize);
    }

    public interface IWordWrapper
    {
        IEnumerable<StringSegment> WordWrap(string input, float lineWidthCorrection = 0);
        float MeasureStringWidthInPixels(string input);
    }

    /// <summary>
    /// Wraps lines to fill a given text surface.
    /// </summary>
    // TODO: Replace this naive implementation with an optimized one once you're not nauseated by the though of writing word wrappers.
    class TextSurfaceWordWrapper : IWordWrapperController
    {
        private IMyTextSurface _surface;
        private float _maxLineLength;
        private string _fontName;
        private float _fontSize;

        private readonly StringBuilder _buffer = new StringBuilder();
        private readonly char[] _wordDelimiters = new[] { ' ', '-', '\n' };

        public IWordWrapperController SetUp(float maxLineLength, IMyTextSurface surface, string fontName, float fontSize)
        {
            _maxLineLength = maxLineLength;
            _surface = surface;
            _fontName = fontName;
            _fontSize = fontSize;
            return this;
        }

        public float MeasureStringWidthInPixels(string input)
        {
            return _surface.MeasureStringInPixels(_buffer.Clear().Append(input), _fontName, _fontSize).X;
        }

        public IEnumerable<StringSegment> WordWrap(string input, float lineWidthCorrection = 0)
        {
            var spaceWidth = _surface.MeasureStringInPixels(_buffer.Clear().Append(' '), _fontName, _fontSize).X;

            int lastWord = 0, nextWord, lastBreak = 0, inputLength = input.Length;
            var lineLength = _maxLineLength + lineWidthCorrection;
            var spaceLeft = lineLength;
           
            while(lastWord < inputLength)
            {
                float wordWidth;
                nextWord = input.IndexOfAny(_wordDelimiters, lastWord);

                if(nextWord == -1)
                {
                    nextWord = inputLength;
                }
                else if (input[nextWord] == '\n') // This is native newline handling... going nuts. Please kill me or tell me there is a cleaner way.
                {
                    _buffer.Clear().Append(input, lastWord, nextWord - lastWord);
                    wordWidth = _surface.MeasureStringInPixels(_buffer, _fontName, _fontSize).X + spaceWidth;
                    if (wordWidth > spaceLeft)
                    {   // Last word before newline doesn't fit; we need an extra line.
                        yield return new StringSegment(input, lastBreak, lastWord - lastBreak);
                        lastBreak = lastWord;
                    }   

                    // Return the current line until the newline, excluding \r if present.
                    yield return new StringSegment(input, lastBreak,
                        input.ElementAtOrDefault(nextWord - 1) == '\r'
                        ? nextWord - 1 - lastBreak
                        : nextWord - lastBreak);

                    spaceLeft = lineLength;
                    lastBreak = lastWord = nextWord + 1; // Jump over the \n we found.

                    if (lastBreak >= inputLength)
                    {   // If the string ends with a newline, we add an empty line, because probably this was the user's intention.
                        yield return new StringSegment(input, inputLength - 1, 0);
                        yield break;
                    }
                    else continue;
                }

                _buffer.Clear().Append(input, lastWord, nextWord - lastWord);
                wordWidth = _surface.MeasureStringInPixels(_buffer, _fontName, _fontSize).X + spaceWidth;

                if (wordWidth > spaceLeft)
                {
                    yield return new StringSegment(input, lastBreak, lastWord - lastBreak);
                    lastBreak = lastWord;
                    spaceLeft = lineLength - wordWidth;
                }
                else
                {
                    spaceLeft -= wordWidth;
                }

                lastWord = nextWord + 1;
            }

            yield return new StringSegment(input, lastBreak, inputLength - lastBreak);
        }
    }
}
