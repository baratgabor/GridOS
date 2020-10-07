using System;

namespace IngameScript
{
    partial class Program
    {
        public struct StringSegment
        {
            public readonly int Start;
            public readonly int Length;

            private string _string;
            private string _stringSegmentCache;

            public StringSegment(string text)
            {
                _string = text;
                Start = 0;
                Length = text.Length;
                _stringSegmentCache = null;
            }

            public StringSegment(string text, int start, int length)
            {
                if (start > text.Length - 1)
                    throw new Exception(nameof(start));

                if (start + length > text.Length)
                    throw new Exception(nameof(length));

                _string = text;
                Start = start;
                Length = length;
                _stringSegmentCache = null;
            }

            public bool IsEmpty => _string == null;
            public bool IsCached => _stringSegmentCache != null;
            public bool IsEndOfString => Start + Length == _string.Length;

            public override string ToString()
            {
                if (_string == null)
                    return null;

                if (_stringSegmentCache != null)
                    return _stringSegmentCache;

                return _stringSegmentCache = _string.Substring(Start, Length);
            }
        }
    }
}
