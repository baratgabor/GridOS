using System.Collections.Generic;

namespace IngameScript
{
    class FontSizeCommand : ConfigurationCommand<float>
    {
        protected override IReadOnlyList<NamedOption<float>> OptionsList => StaticConfig.FontSizes.AsList;
        protected override string LabelFormatString => "Font size: {0}";

        public FontSizeCommand(IMenuInstanceServices menuServices) : base(menuServices)
        {}

        protected override float GetInitialValue()
            => _menuServices.DisplayConfig.BaseFontSize;

        protected override void SetNewValue(float value)
            => _menuServices.SetFontSize(value);
    }
}
