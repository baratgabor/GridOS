namespace IngameScript
{
    class FontSizeCommand : MenuCommand
    {
        private readonly IDisplayConfig _displayConfig;
        private readonly IMenuInstanceServices _menuServices;
        private int _selectedIndex;

        public FontSizeCommand(IMenuInstanceServices menuServices) : base("", null)
        {
            _menuServices = menuServices;
            _displayConfig = menuServices.DisplayConfig;

            _command = SwitchFontSize;
            SetLabel(_displayConfig.FontSize);

            _selectedIndex = StaticConfig.FontSizes.IndexOf(_displayConfig.FontSize);
            if (_selectedIndex == -1) _selectedIndex = 0;
        }

        private void SetLabel(float fontSize)
        {
            Label = $"Font size: {fontSize * 100}%";
        }

        private void SwitchFontSize()
        {
            _selectedIndex = ++_selectedIndex % StaticConfig.FontSizes.Count;
            var newFontSize = StaticConfig.FontSizes[_selectedIndex];
            
            SetLabel(newFontSize);
            _menuServices.SetFontSize(newFontSize);
        }
    }
}
