namespace IngameScript
{
    class BackgroundColorCommand : MenuCommand
    {
        private readonly IMenuInstanceServices _menuServices;

        private int _selectedColorIndex;

        public BackgroundColorCommand(IMenuInstanceServices menuServices) : base("", null)
        {
            _command = SwitchColor;
            _menuServices = menuServices;

            SetLabel();
        }

        private void SetLabel()
        {
            Label = $"Background color: {StaticConfig.BackgroundColors[_selectedColorIndex].Name}";
        }

        private void SwitchColor()
        {
            _selectedColorIndex = ++_selectedColorIndex % StaticConfig.BackgroundColors.Count;
            SetLabel();
            _menuServices.SetBackgroundColor(StaticConfig.BackgroundColors[_selectedColorIndex].Color);
        }

        struct ColorSetting
        {
            public string Name;
            public Color Color;
        }
    }
}
