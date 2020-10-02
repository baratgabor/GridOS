using System.Collections.Generic;
using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program
    {
        public class StringHelpers
        {
            public static IEnumerable<StringSegment> WordWrap(string input, int maxLineLength, char[] breakableCharacters)
            {
                int lastBreakIndex = 0;

                while (true)
                {
                    var nextForcedLineBreak = lastBreakIndex + maxLineLength;

                    // If there are native new lines in the range of the next line (or the remainder), yield all new lines.
                    int nativeNewlineIndex = input.IndexOf(Environment.NewLine, lastBreakIndex, Math.Min(nextForcedLineBreak, input.Length - lastBreakIndex));
                    if (nativeNewlineIndex > -1)
                    {
                        yield return new StringSegment(input, lastBreakIndex, nativeNewlineIndex - lastBreakIndex);
                        lastBreakIndex = nativeNewlineIndex + Environment.NewLine.Length;
                        continue;
                    }

                    // If the remainder is shorter than the allowed line-length, return the remainder. Short-circuits instantly for strings shorter than line-length.
                    if (nextForcedLineBreak >= input.Length)
                    {
                        yield return new StringSegment(input, lastBreakIndex, input.Length - lastBreakIndex);
                        yield break;
                    }

                    // Find the last breakable point preceding the next forced break position.
                    var nextBreakIndex = input.LastIndexOfAny(breakableCharacters, nextForcedLineBreak, maxLineLength) + 1;

                    // If there is no breakable point, which means a word is longer than line length, force-break it.
                    if (nextBreakIndex == 0)
                    {
                        nextBreakIndex = nextForcedLineBreak;
                    }

                    yield return new StringSegment(input, lastBreakIndex, nextBreakIndex - lastBreakIndex);

                    lastBreakIndex = nextBreakIndex;
                }
            }
        }
    }
}
