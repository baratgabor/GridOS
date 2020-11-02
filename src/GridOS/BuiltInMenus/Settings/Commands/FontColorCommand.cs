using System.Collections.Generic;

namespace IngameScript
{
    class FontColorCommand : ConfigurationCommand<Color>
    {
        protected override IReadOnlyList<NamedOption<Color>> OptionsList => StaticConfig.FontColors.AsList2;
        protected override string LabelFormatString => "Font color: {0}";

        public FontColorCommand(IMenuInstanceServices menuService) : base(menuService)
        {}

        protected override Color GetInitialValue() => _menuServices.DisplayConfig.BaseFontColor;
        protected override void SetNewValue(Color value) => _menuServices.SetFontColor(value);
    }
}
