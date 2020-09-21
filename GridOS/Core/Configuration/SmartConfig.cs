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
                Element = " ",
                Command = "·",
                Group = "·"
            };

            public Affix Suffixes_Unselected { get; set; } = new Affix()
            {
                Element = "",
                Command = "",
                Group = "»"
            };
            public Affix Prefixes_Selected { get; set; } = new Affix()
            {
                Element = " ",
                Command = "•",
                Group = "•"
            };
            public Affix Suffixes_Selected { get; set; } = new Affix()
            {
                Element = "",
                Command = "",
                Group = "»"
            };

            public string GetPrefixFor(IDisplayElement element, bool selected)
            {
                if (selected) return GetAffix(element, selected, Prefixes_Selected);
                else return GetAffix(element, selected, Prefixes_Unselected);
            }

            public string GetSuffixFor(IDisplayElement element, bool selected)
            {
                if (selected) return GetAffix(element, selected, Suffixes_Selected);
                else return GetAffix(element, selected, Suffixes_Unselected);
            }

            protected string GetAffix(IDisplayElement element, bool selected, Affix affix)
            {
                string value = "";

                if (element is IDisplayGroup)
                    value = affix.Group;
                else if (element is IDisplayCommand)
                    value = affix.Command;
                else
                    value = affix.Element;

                return value;
            }

            public struct Affix
            {
                public string Group;
                public string Command;
                public string Element;
            }
        }
    }
}
