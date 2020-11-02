using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly char[] _wordDelimiters = new[] { ' ', '-', '\n' };

        public TextSurfaceWordWrapper(IDisplayConfig config)
        {
            _config = config;
            _surface = config.OutputSurface;
        }

        public float MeasureStringWidthInPixels(string input)
        {
            return _surface.MeasureStringInPixels(_buffer.Clear().Append(input), _config.BaseFontName, _config.BaseFontSize).X;
        }

        public IEnumerable<StringSegment> WordWrap(string input, float maxWidthCorrection)
        {
            var fontSize = _config.BaseFontSize;
            var fontName = _config.BaseFontName;
            var lineWidth = _config.OutputWidth + maxWidthCorrection;
            var spaceWidth = _surface.MeasureStringInPixels(_buffer.Clear().Append(' '), fontName, fontSize).X;

            int lastWord = 0, nextWord, lastBreak = 0, inputLength = input.Length;
            var spaceLeft = lineWidth;
           
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
                    wordWidth = _surface.MeasureStringInPixels(_buffer, fontName, fontSize).X + spaceWidth;
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

                    spaceLeft = lineWidth;
                    lastBreak = lastWord = nextWord + 1; // Jump over the \n we found.

                    if (lastBreak >= inputLength)
                    {   // If the string ends with a newline, we add an empty line, because probably this was the user's intention.
                        yield return new StringSegment(input, inputLength - 1, 0);
                        yield break;
                    }
                    else continue;
                }

                _buffer.Clear().Append(input, lastWord, nextWord - lastWord);
                wordWidth = _surface.MeasureStringInPixels(_buffer, fontName, fontSize).X + spaceWidth;

                if (wordWidth > spaceLeft)
                {
                    yield return new StringSegment(input, lastBreak, lastWord - lastBreak);
                    lastBreak = lastWord;
                    spaceLeft = lineWidth - wordWidth;
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
