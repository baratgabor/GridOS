namespace IngameScript
{
    class FontColorCommand : MenuCommand
    {
        private readonly IMenuInstanceServices _menuServices;

        private int _selectedIndex;

        public FontColorCommand(IMenuInstanceServices menuServices) : base("", null)
        {
            _command = SwitchFontColor;
            _menuServices = menuServices;

            SetLabel();
        }
        
        private void SetLabel()
        {
            Label = $"Font color: {StaticConfig.FontColors[_selectedIndex].Name}";
        }

        private void SwitchFontColor()
        {
            _selectedIndex = ++_selectedIndex % StaticConfig.FontColors.Count;
            var color = StaticConfig.FontColors[_selectedIndex].Color;
            Color.Multiply(color, 0.5f);
            SetLabel();
            _menuServices.SetFontColor(color);
        }

        struct ColorSetting
        {
            public string Name;
            public Color Color;
        }
    }
}
