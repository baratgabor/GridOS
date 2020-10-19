using System;
using System.Collections.Generic;

namespace IngameScript
{
    class MenuLineGenerator
    {
        private readonly IMenuPresentationConfig _config;
        private readonly IWordWrapper _wordWrapper;
        private readonly string _leftPadding;
        private readonly List<MenuLine> _menuLineBuffer = new List<MenuLine>();
        private readonly List<StringSegment> _segmentBuffer = new List<StringSegment>();
        private readonly float _linePrefixInPixels;

        public MenuLineGenerator(IMenuPresentationConfig presentationConfig, IWordWrapper wordWrapper)
        {
            _config = presentationConfig;
            _wordWrapper = wordWrapper;
            _leftPadding = new string(_config.PaddingChar, _config.PaddingLeft);
            _linePrefixInPixels = wordWrapper.MeasureStringWidthInPixels(_leftPadding + "  "); // TODO: Find a cleaner solution to incorporate prefix correction into line length calculation.
        }

        public IEnumerable<MenuLine> StreamLines(IMenuItem item)
        {
            var lineIndex = 0;

            foreach (var line in _wordWrapper.WordWrap(item.Label, -_linePrefixInPixels))
            {
                yield return CreateMenuLine(item, line, lineIndex);

                lineIndex++;
            }
        }

        public IReadOnlyList<MenuLine> GetLines(IMenuItem item, int takeLast = int.MaxValue)
        {
            _menuLineBuffer.Clear();
            _segmentBuffer.Clear();

            _segmentBuffer.AddRange(
                _wordWrapper.WordWrap(item.Label, -_linePrefixInPixels)
            );

            var segmentCount = _segmentBuffer.Count;
            var startIndex = Math.Max(0, _segmentBuffer.Count - takeLast);

            for (int i = startIndex; i < segmentCount; i++)
            {
                var line = _segmentBuffer[i];
                _menuLineBuffer.Add(
                    CreateMenuLine(item, line, i));
            }

            return _menuLineBuffer;
        }

        private MenuLine CreateMenuLine(IMenuItem item, StringSegment itemLine, int lineIndex)
        {
            return new MenuLine()
            {
                BackingMenuItem = item,
                LeftPadding = _leftPadding,
                UnselectedPrefix = GetUnselectedPrefixFor(item, lineIndex),
                SelectedPrefix = GetSelectedPrefixFor(item, lineIndex),
                Suffix = GetSuffixFor(item, itemLine.IsEndOfString),
                MenuItemString = item.Label,
                LineStartIndex = itemLine.Start,
                LineLength = itemLine.Length,
                LineIndex = lineIndex
            };
        }

        private char GetSelectedPrefixFor(IMenuItem item, int lineIndex)
        {
            char prefix;

            if (lineIndex > 0)
                prefix = _config.SelectionMarker;
            else if (item is IMenuGroup)
                prefix = _config.Prefixes_Selected.Group;
            else if (item is IMenuCommand)
                prefix = _config.Prefixes_Selected.Command;
            else
                prefix = _config.Prefixes_Selected.Item;

            if (prefix == ' ') // Lack of special selection prefix still should display line selection marker.
                return _config.SelectionMarker;

            return prefix;
        }

        private char GetUnselectedPrefixFor(IMenuItem item, int lineIndex)
        {
            if (lineIndex > 0)
                return ' ';
            if (item is IMenuGroup)
                return _config.Prefixes_Unselected.Group;
            if (item is IMenuCommand)
                return _config.Prefixes_Unselected.Command;
            
            return _config.Prefixes_Unselected.Item;
        }

        private char GetSuffixFor(IMenuItem item, bool isEndOfString)
        {
            if (!isEndOfString)
                return ' ';
            if (item is IMenuGroup)
                return _config.Suffixes.Group;
            if (item is IMenuCommand)
                return _config.Suffixes.Command;

            return _config.Suffixes.Item;
        }
    }
}
