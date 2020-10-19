using System.Collections.Generic;
using System.Text;
using System;

namespace IngameScript
{
    /// <summary>
    /// Sets up a TextSurface with the appropriate settings, and fills/refreshes it with the content of the Controls added.
    /// </summary>
    class DisplayView : IView
    {
        protected IMyTextSurface _surface;

        protected bool _contentDirty = true;
        protected StringBuilder _content = new StringBuilder();
        protected List<IControl> _controls = new List<IControl>();

        protected const char _lineSeparatorCharTop = '.';
        protected const char _lineSeparatorCharBottom = '˙';
        protected MainConfig _config;

        public DisplayView(IMyTextSurface surface, MainConfig config)
        {
            _surface = surface;
            _config = config;

            SetupSurface();
            AdaptToSurface();
        }

        public DisplayView AddControl(IControl control)
        {
            _controls.Add(control);
            control.RedrawRequired += OnRedrawRequired;
            return this;
        }

        // TODO: Consider control disposal mechanisms? Or what's the assumption when we remove a control?
        public void RemoveControl(IControl control)
        {
            if (_controls.Contains(control))
            {
                control.RedrawRequired -= OnRedrawRequired;
                _controls.Remove(control);
            }
        }

        public void ClearControls()
        {
            foreach (var c in _controls)
                c.RedrawRequired -= OnRedrawRequired;

            _controls.Clear();
        }

        public void Redraw()
        {
            if (!_contentDirty)
                return;

            _content.Clear();

            for (int i = 0; i < _controls.Count; i++)
            {
                _content.Append(_controls[i].GetContent());
                _content.AppendLine();
            }

            _content.Length -= Environment.NewLine.Length; // Trim last newline.

            _surface.WriteText(_content.ToString());

            _contentDirty = false;
        }

        private void AdaptToSurface()
        {
            _config.OutputSurface = _surface;
            _config.OutputWidth = _surface.TextureSize.X * 0.85f;
            _config.OutputHeight = _surface.TextureSize.Y * 0.95f;
            var charHeight = _surface.MeasureStringInPixels(_content.Clear().Append("X"), _config.FontName, _config.FontSize).Y;
            _config.OutputLineCapacity = (int)(_config.OutputHeight / charHeight);

            _config.MenuLines = Math.Max(3, _config.OutputLineCapacity - 4); // TODO: Express this properly. Header, etc. uses up 4 lines, so less is left for menu. And menu is min 3 lines.

            _config.PaddingLeft = 1;

            _config.SeparatorLineTop = new string(
                _lineSeparatorCharTop,
                (int)(_config.OutputWidth / _surface.MeasureStringInPixels(
                    _content.Clear().Append(_lineSeparatorCharTop), _config.FontName, _config.FontSize).X));

            _config.SeparatorLineBottom = new string(
                _lineSeparatorCharBottom,
                (int)(_config.OutputWidth / _surface.MeasureStringInPixels(
                    _content.Clear().Append(_lineSeparatorCharBottom), _config.FontName, _config.FontSize).X));
        }

        private void SetupSurface()
        {
            _surface.Font = _config.FontName;
            _surface.FontSize = _config.FontSize;
            _surface.ContentType = ContentType.TEXT_AND_IMAGE;
            _surface.FontColor = _config.FontColor;
            _surface.BackgroundColor = _config.BackgroundColor;
            _surface.TextPadding = 4f;
        }

        private void OnRedrawRequired(IControl control)
        {
            _contentDirty = true;
        }

        public void SetFontSize(float fontSize)
        {
            if (fontSize < 0 || fontSize > 10)
                throw new Exception("Font size must be between 0 and 10.");

            _config.FontSize = fontSize;
            SetupSurface();
            AdaptToSurface();
            Redraw();
        }

        public void SetFontColor(Color color)
        {
            _config.FontColor = color;
            SetupSurface();
        }

        public void SetBackgroundColor(Color color)
        {
            _config.BackgroundColor = color;
            SetupSurface();
        }
    }
}
