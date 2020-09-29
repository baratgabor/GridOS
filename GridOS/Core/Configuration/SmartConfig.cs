namespace IngameScript
{
    partial class Program
    {
        class SmartConfig : IWordWrappingConfig, IViewportConfig, IPaddingConfig, INavConfig, IAffixConfig, IBreadcrumbConfig, IViewConfig_Writeable
        {
            public int LineLength { get; set; }
            public char[] Terminators { get; set; } = { ' ', '-' };

            public int LineHeight { get; set; }

            public char PaddingChar { get; set; } = ' ';
            public int PaddingLeft { get; set; } = 0;
            public int PaddingLeft_FirstLine { get; set; }

            public string PathSeparator { get; set; } = "›";
            public string SeparatorLineTop { get; set; }
            public string SeparatorLineBottom { get; set; }

            public char SelectionMarker { get; set; } = '›';

            public Affix Prefixes_Unselected { get; set; } = new Affix()
            {
                Item = ' ',
                Command = '·',
                Group = '·'
            };

            public Affix Suffixes_Unselected { get; set; } = new Affix()
            {
                Item = ' ',
                Command = ' ',
                Group = '»'
            };
            public Affix Prefixes_Selected { get; set; } = new Affix()
            {
                Item = ' ',
                Command = '•',
                Group = '•'
            };
            public Affix Suffixes_Selected { get; set; } = new Affix()
            {
                Item = ' ',
                Command = ' ',
                Group = '»'
            };

            public char GetPrefixFor(IMenuItem item, bool selected)
            {
                if (selected) return GetAffix(item, selected, Prefixes_Selected);
                else return GetAffix(item, selected, Prefixes_Unselected);
            }

            public char GetSuffixFor(IMenuItem item, bool selected)
            {
                if (selected) return GetAffix(item, selected, Suffixes_Selected);
                else return GetAffix(item, selected, Suffixes_Unselected);
            }

            protected char GetAffix(IMenuItem item, bool selected, Affix affix)
            {
                char value;
                if (item is IMenuGroup)
                    value = affix.Group;
                else if (item is IMenuCommand)
                    value = affix.Command;
                else
                    value = affix.Item;

                return value;
            }

            public struct Affix
            {
                public char Group;
                public char Command;
                public char Item;
            }
        }
    }
}
