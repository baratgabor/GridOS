namespace IngameScript
{
    class MainConfig : IMenuPresentationConfig, IBreadcrumbConfig, IDisplayConfig
    {
        public string PathSeparator { get; set; } = "›";
        public string SeparatorLineTop { get; set; }
        public string SeparatorLineBottom { get; set; }

        public int MenuLines { get; set; }
        public char PaddingChar { get; set; } = ' ';
        public int PaddingLeft { get; set; } = 0;
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

        public string FontName { get; set; } = "Debug";
        public float FontSize { get; set; } = 1.25f;
        public Color FontColor { get; set; } = new Color() { R = 0, G = 77, B = 90, A = 255 };
        public Color BackgroundColor { get; set; } = new Color() { R = 0, G = 12, B = 15, A = 255 };
        public IMyTextSurface OutputSurface { get; set; }
        public float OutputWidth { get; set; }
        public float OutputHeight { get; set; }
        public int OutputLineCapacity { get; set; }
    }
}
