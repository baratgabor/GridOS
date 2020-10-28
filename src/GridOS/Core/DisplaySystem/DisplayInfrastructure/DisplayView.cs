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

        protected RectangleF _viewport;

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

        public void Redraw(bool flush = false)
        {
            if (!flush && !_contentDirty)
                return;

            _content.Clear();

            for (int i = 0; i < _controls.Count; i++)
            {
                _content.Append(_controls[i].GetContent(flush));
                _content.AppendLine();
            }

            _content.Length -= Environment.NewLine.Length; // Trim last newline.

            WriteContentAsSprite(_content.ToString());

            _contentDirty = false;
        }

        private void WriteContentAsSprite(string content)
        {
            using (var frame = _surface.DrawFrame())
            {
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Color = _config.BackgroundColor,
                    Alignment = TextAlignment.CENTER,
                    Position = _viewport.Center,
                    Size = _surface.SurfaceSize
                });

                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = content,
                    RotationOrScale = _config.FontSize,
                    Color = _config.FontColor,
                    Alignment = TextAlignment.LEFT,
                    FontId = _config.FontName,
                    Position = _viewport.Position
                });
            }
        }

        private void AdaptToSurface()
        {
            _viewport = new RectangleF(
                (_surface.TextureSize - _surface.SurfaceSize) / 2f,
                _surface.SurfaceSize
            );

            _config.OutputSurface = _surface;
            _config.OutputWidth = _surface.SurfaceSize.X;
            _config.OutputHeight = _surface.SurfaceSize.Y;
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
            _surface.ContentType = ContentType.SCRIPT;
            _surface.Script = "";
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
            Redraw(flush: true);
        }

        public void SetFontColor(Color color)
        {
            _config.FontColor = color;
            SetupSurface();
            Redraw(flush: true);
        }

        public void SetBackgroundColor(Color color)
        {
            _config.BackgroundColor = color;
            SetupSurface();
            Redraw(flush: true);
        }
    }
}
