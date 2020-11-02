using System.Collections.Generic;

namespace IngameScript
{
    /// <summary>
    /// Base class for making concrete setting commands that switch between values in a list of predefined options.
    /// </summary>
    /// <typeparam name="T">Type of the setting value.</typeparam>
    abstract class ConfigurationCommand<T> : MenuCommand
    {
        protected abstract IReadOnlyList<NamedOption<T>> OptionsList { get; }
        protected abstract string LabelFormatString { get; }

        private int _selectedIndex;
        protected readonly IMenuInstanceServices _menuServices;

        public ConfigurationCommand(IMenuInstanceServices menuServices) : base("", null)
        {
            _menuServices = menuServices;
            _command = SwitchValue;

            var currentValue = GetInitialValue();
            _selectedIndex = OptionsList.FindIndex(x => x.Value.Equals(currentValue), notFoundValue: 0);
            SetLabel();
        }

        private void SwitchValue()
        {
            _selectedIndex = ++_selectedIndex % OptionsList.Count;
            SetLabel();
            SetNewValue(OptionsList[_selectedIndex].Value);
        }

        private void SetLabel()
        {
            Label = string.Format(LabelFormatString, OptionsList[_selectedIndex].Name);
        }

        protected abstract void SetNewValue(T value);
        protected abstract T GetInitialValue();
    }
}
