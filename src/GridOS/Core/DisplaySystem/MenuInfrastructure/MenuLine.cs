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
        public char UnselectedPrefix;
        public char SelectedPrefix;
        public string MenuItemString;
        public int LineStartIndex;
        public int LineLength;
        public char Suffix;
        public int LineIndex;

        public void AppendTo(StringBuilder stringBuilder, bool isSelected)
        {
            stringBuilder.Append(LeftPadding);

            if (isSelected)
                stringBuilder.Append(SelectedPrefix);
            else
                stringBuilder.Append(UnselectedPrefix);

            stringBuilder.Append(' ');
            stringBuilder.Append(MenuItemString, LineStartIndex, LineLength);

            if (Suffix != ' ')
            {
                stringBuilder.Append(' ');
                stringBuilder.Append(Suffix);
            }
        }

        public bool IsEndOfMenuItem()
        {
            return LineStartIndex + LineLength == MenuItemString.Length;
        }
    }
}
