using System.Collections.Generic;

namespace IngameScript
{
    class BackgroundColorCommand : ConfigurationCommand<Color>
    {
        protected override IReadOnlyList<NamedOption<Color>> OptionsList => StaticConfig.BackgroundColors.AsList2;
        protected override string LabelFormatString => "Background color: {0}";

        public BackgroundColorCommand(IMenuInstanceServices menuServices) : base(menuServices)
        {}

        protected override Color GetInitialValue()
            => _menuServices.DisplayConfig.BaseBackgroundColor;
        protected override void SetNewValue(Color value)
            => _menuServices.SetBackgroundColor(value);
    }
}
