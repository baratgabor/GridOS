namespace IngameScript
{
    /// <summary>
    /// Exposes features tied to individual menu instances on different displays.
    /// </summary>
    public interface IMenuInstanceServices
    {
        IDisplayConfig DisplayConfig { get; }
        IMenuPresentationConfig MenuConfig { get; }
        void SetFontSize(float fontSize);
        void SetFontColor(Color color);
        void SetBackgroundColor(Color color);
    }
}
