using System.Text;

namespace IngameScript
{
    /// <summary>
    /// Represents a line on the menu screen. Facilitates adding menu line to a StringBuilder without string allocation.
    /// </summary>
    struct MenuLine
    {
        public IMenuItem BackingMenuItem;
        public string LeftPadding;
        public char Prefix;
        public string MenuItemString;
        public int LineStartIndex;
        public int LineLength;
        public char Suffix;
        public int LineIndex;
        public char SelectionMarker;

        public MenuLine(IMenuItem backingMenuItem, string leftPadding, char prefix, string menuItemString, int lineStartIndex, int lineLength, char suffix, char selectionMarker = '›', int lineOffset = 0)
        {
            BackingMenuItem = backingMenuItem;
            LeftPadding = leftPadding;
            Prefix = prefix;
            MenuItemString = menuItemString;
            LineStartIndex = lineStartIndex;
            LineLength = lineLength;
            Suffix = suffix;
            SelectionMarker = selectionMarker;
            LineIndex = lineOffset;
        }

        public void AppendTo(StringBuilder stringBuilder, bool isSelected)
        {
            stringBuilder.Append(LeftPadding);
            stringBuilder.Append(isSelected ? SelectionMarker : ' ');
            stringBuilder.Append(Prefix + " ");
            stringBuilder.Append(MenuItemString, LineStartIndex, LineLength);
            stringBuilder.Append(" " + Suffix);
        }

        public bool IsEndOfMenuItem()
        {
            return LineStartIndex + LineLength == MenuItemString.Length;
        }
    }
}
