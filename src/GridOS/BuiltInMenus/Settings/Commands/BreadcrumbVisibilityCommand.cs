namespace IngameScript
{
    class BreadcrumbVisibilityCommand : MenuCommand
    {
        private readonly IMenuInstanceServices _services;

        public BreadcrumbVisibilityCommand(IMenuInstanceServices services) : base("", null)
        {
            _command = Switch;
            _services = services;
            SetLabel(_services.GetBreadcrumbVisiblity());
        }

        public void SetLabel(bool isVisible)
        {
            if (isVisible) Label = "Breadcrumb bar: Visible";
            else Label = "Breadcrumb bar: Hidden";
        }

        public void Switch()
        {
            var newVisibility = !_services.GetBreadcrumbVisiblity();
            _services.SetBreadcrumbVisibility(newVisibility);
            SetLabel(newVisibility);
        }
    }
}
