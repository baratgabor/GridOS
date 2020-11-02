using System.Collections.Generic;

namespace IngameScript
{
    class FontTypeCommand : ConfigurationCommand<string>
    {
        protected override IReadOnlyList<NamedOption<string>> OptionsList => StaticConfig.FontTypes.AsList;
        protected override string LabelFormatString => "Font: {0}";

        public FontTypeCommand(IMenuInstanceServices menuServices) : base(menuServices)
        {}

        protected override string GetInitialValue()
            => _menuServices.DisplayConfig.BaseFontName;

        protected override void SetNewValue(string value)
            => _menuServices.SetFontType(value);
    }
}
