using System;
using System.Collections.Generic;

namespace IngameScript
{
    // TODO: Finalize setting change notification implementation. Decide what way to go with configuration; separate static from per display instance. Try to move defaults to some plain DTO.
    class MainConfig : IMenuPresentationConfig, IBreadcrumbConfig, IDisplayConfig
    {
        public string PathSeparator { get; set; } = "›";
        public string SeparatorLineTop { get; set; }
        public string SeparatorLineBottom { get; set; }

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

        private int menuLines;
        private string fontName = "Debug";
        private float fontSize = StaticConfig.FontSizes[2];
        private Color fontColor = StaticConfig.FontColors[0].Color;
        private Color backgroundColor = StaticConfig.BackgroundColors[0].Color;
        private float outputWidth;
        private int outputLineCapacity;
        private float outputHeight;

        public int MenuLines { get { return menuLines; } set { SetAndNotify(ref menuLines, nameof(MenuLines), value); } }

        public string FontName { get { return fontName; } set { SetAndNotify(ref fontName, nameof(FontName), value); } }
        public float FontSize { get { return fontSize; } set { SetAndNotify(ref fontSize, nameof(FontSize), value); } }
        public Color FontColor { get { return fontColor; } set { SetAndNotify(ref fontColor, nameof(FontColor), value); } }
        public Color BackgroundColor { get { return backgroundColor; } set { SetAndNotify(ref backgroundColor, nameof(BackgroundColor), value); } }
        public IMyTextSurface OutputSurface { get; set; }
        public float OutputWidth { get { return outputWidth; } set { SetAndNotify(ref outputWidth, nameof(OutputWidth), value); } }
        public float OutputHeight { get { return outputHeight; } set { SetAndNotify(ref outputHeight, nameof(OutputHeight), value); } }
        public int OutputLineCapacity { get { return outputLineCapacity; } set { SetAndNotify(ref outputLineCapacity, nameof(OutputLineCapacity), value); } }

        public event Action<string> SettingChanged;
        protected void SetAndNotify<T>(ref T field, string fieldName, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            SettingChanged?.Invoke(fieldName);
            return;
        }
    }
}
