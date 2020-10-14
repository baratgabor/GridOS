namespace IngameScript
{
    class MainConfig : IMenuPresentationConfig, IBreadcrumbConfig, IViewConfig_Writeable
    {
        public int LineLength { get; set; }
        public char[] WordDelimiters { get; set; } = { ' ', '-' };

        public int LineHeight { get; set; }

        public char PaddingChar { get; set; } = ' ';
        public int PaddingLeft { get; set; } = 0;

        public string PathSeparator { get; set; } = "›";
        public string SeparatorLineTop { get; set; }
        public string SeparatorLineBottom { get; set; }

        public char SelectionMarker { get; set; } = '›';

        public AffixConfig Prefixes_Unselected { get; set; } = new AffixConfig()
        {
            Item = ' ',
            Command = '·',
            Group = '·'
        };

        public AffixConfig Prefixes_Selected { get; set; } = new AffixConfig()
        {
            Item = ' ',
            Command = '•',
            Group = '•'
        };

        public AffixConfig Suffixes { get; set; } = new AffixConfig()
        {
            Item = ' ',
            Command = ' ',
            Group = '»'
        };
    }
}
