using System;

namespace IngameScript
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
            if (length < 0)
                throw new Exception($"{nameof(length)} must be zero or positive.");

            if (start < 0 || start > text.Length - 1)
                throw new Exception($"{nameof(start)} must be a valid index.");

            if (start + length > text.Length)
                throw new Exception($"{nameof(length)}+{nameof(length)} must be a valid index.");

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
