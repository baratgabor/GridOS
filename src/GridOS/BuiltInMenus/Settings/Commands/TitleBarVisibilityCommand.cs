namespace IngameScript
{
    class TitleBarVisibilityCommand : MenuCommand
    {
        private readonly IMenuInstanceServices _services;

        public TitleBarVisibilityCommand(IMenuInstanceServices services) : base("", null)
        {
            _command = Switch;
            _services = services;
            SetLabel(_services.GetTitleBarVisiblity());
        }

        public void SetLabel(bool isVisible)
        {
            if (isVisible) Label = "Title bar: Visible";
            else Label = "Title bar: Hidden";
        }

        public void Switch()
        {
            var newVisibility = !_services.GetTitleBarVisiblity();
            _services.SetTitleBarVisiblity(newVisibility);
            SetLabel(newVisibility);
        }
    }
}
