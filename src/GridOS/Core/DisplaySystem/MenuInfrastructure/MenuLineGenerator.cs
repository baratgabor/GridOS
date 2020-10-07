using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        class MenuLineGenerator
        {
            private readonly IMenuPresentationConfig _config;
            private readonly string _leftPadding;
            private readonly int _lineLength;
            private readonly List<MenuLine> _menuLineBuffer = new List<MenuLine>();
            private readonly List<StringSegment> _segmentBuffer = new List<StringSegment>();

             public MenuLineGenerator
                (IMenuPresentationConfig presentationConfig)
            {
                _config = presentationConfig;

                _lineLength = _config.LineLength - 2 - _config.PaddingLeft;
                _leftPadding = new string(_config.PaddingChar, _config.PaddingLeft);
            }

            public IEnumerable<MenuLine> StreamLines(IMenuItem item)
            {
                var counter = 0;

                foreach (var line in StringHelpers.WordWrap(item.Label, _lineLength, _config.WordDelimiters))
                {
                    yield return new MenuLine()
                    {
                        BackingMenuItem = item,
                        LeftPadding = _leftPadding,
                        Prefix = line.Start == 0 ? _config.GetPrefixFor(item, false) : ' ',
                        Suffix = line.IsEndOfString ? _config.GetSuffixFor(item, false) : ' ',
                        MenuItemString = item.Label,
                        LineStartIndex = line.Start,
                        LineLength = line.Length,
                        LineIndex = counter,
                        SelectionMarker = _config.SelectionMarker
                    };

                    counter++;
                }
            }

            public IReadOnlyList<MenuLine> GetLines(IMenuItem item, int takeLast = int.MaxValue)
            {
                _menuLineBuffer.Clear();
                _segmentBuffer.Clear();

                _segmentBuffer.AddRange(
                    StringHelpers.WordWrap(item.Label, _lineLength, _config.WordDelimiters)
                );

                var segmentCount = _segmentBuffer.Count;
                var startIndex = Math.Max(0, _segmentBuffer.Count - takeLast);

                for (int i = startIndex; i < segmentCount; i++)
                {
                    var line = _segmentBuffer[i];
                    _menuLineBuffer.Add(new MenuLine()
                    {
                        BackingMenuItem = item,
                        LeftPadding = _leftPadding,
                        Prefix = line.Start == 0 ? _config.GetPrefixFor(item, false) : ' ',
                        Suffix = line.IsEndOfString ? _config.GetSuffixFor(item, false) : ' ',
                        MenuItemString = item.Label,
                        LineStartIndex = line.Start,
                        LineLength = line.Length,
                        LineIndex = i,
                        SelectionMarker = _config.SelectionMarker
                    });
                }

                return _menuLineBuffer;
            }
        }
    }
}
