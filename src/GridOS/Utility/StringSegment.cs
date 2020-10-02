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
                _string = text;
                Start = start;
                Length = length;
                _stringSegmentCache = null;
            }

            public bool IsEmpty => _string == null;
            public bool IsCached => _stringSegmentCache != null;

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
