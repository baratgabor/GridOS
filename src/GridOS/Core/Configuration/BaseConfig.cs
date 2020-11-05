using System;
using System.Collections.Generic;

namespace IngameScript
{
    // TODO: Finalize setting change notification implementation. Decide what way to go with configuration; separate static from per display instance. Try to move defaults to some plain DTO.
    class BaseConfig : IMenuPresentationConfig, IBreadcrumbConfig, IDisplayConfig
    {
        public string PathSeparator { get; set; } = "›";

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

        private string baseFontName = StaticConfig.FontTypes.Outlined;
        private float baseFontSize = StaticConfig.FontSizes.Percent100;
        private Color baseFontColor = StaticConfig.FontColors.White;
        private Color baseBackgroundColor = StaticConfig.BackgroundColors.Cyan;
        private float baseLineHeight;
        private float baseLineSpacing;

        private float outputWidth;
        private float outputHeight;

        public string BaseFontName { get { return baseFontName; } set { SetAndNotify(ref baseFontName, nameof(BaseFontName), value); } }
        public float BaseFontSize { get { return baseFontSize; } set { SetAndNotify(ref baseFontSize, nameof(BaseFontSize), value); } }
        public Color BaseFontColor { get { return baseFontColor; } set { SetAndNotify(ref baseFontColor, nameof(BaseFontColor), value); } }
        public Color BaseBackgroundColor { get { return baseBackgroundColor; } set { SetAndNotify(ref baseBackgroundColor, nameof(BaseBackgroundColor), value); } }
        public float BaseLineHeight { get { return baseLineHeight; } set { SetAndNotify(ref baseLineHeight, nameof(BaseLineHeight), value); } }
        public float BaseLineSpacing { get { return baseLineSpacing; } set { SetAndNotify(ref baseLineSpacing, nameof(BaseLineSpacing), value); } }

        public IMyTextSurface OutputSurface { get; set; }
        public float OutputWidth { get { return outputWidth; } set { SetAndNotify(ref outputWidth, nameof(OutputWidth), value); } }
        public float OutputHeight { get { return outputHeight; } set { SetAndNotify(ref outputHeight, nameof(OutputHeight), value); } }
        public string DisplayId { get; set; }

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
