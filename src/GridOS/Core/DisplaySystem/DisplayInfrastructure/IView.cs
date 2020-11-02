namespace IngameScript
{
    interface IView
    {
        DisplayView AddControl(IControl control);
        void RemoveControl(IControl control);
        void ClearControls();
        void Redraw(bool flush = false);
        void SetFontType(string fontName);
        void SetFontSize(float fontSize);
        void SetFontColor(Color color);
        void SetBackgroundColor(Color color);
    }
}
