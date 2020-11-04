using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

namespace IngameScript
{
    /// <summary>
    /// Sets up a TextSurface with the appropriate settings, and fills/refreshes it with the content of the Controls added.
    /// </summary>
    class DisplayView : IView
    {
        protected bool _contentDirty = true;
        protected RectangleF _viewport;

        protected readonly List<KeyValuePair<IControl, List<MySprite>>> _content = new List<KeyValuePair<IControl, List<MySprite>>>();
        protected readonly IWordWrapperController _wordWrapper;
        protected readonly IMyTextSurface _surface;
        protected readonly BaseConfig _config;

        private readonly StringBuilder _buffer = new StringBuilder();

        public DisplayView(IMyTextSurface surface, BaseConfig config, IWordWrapperController wordWrapper)
        {
            _surface = surface;
            _config = config;
            _wordWrapper = wordWrapper;

            SetupSurface();
            AdaptToSurface();
        }

        public void Dispose()
        {
            ClearControls();
            DecommissionSurface();
        }

        public DisplayView AddControl(IControl control)
        {
            if (_content.Any(x => x.Key == control))
                throw new Exception("Control instance already added.");

            control.RedrawRequired += OnRedrawRequired;
            _content.Add(new KeyValuePair<IControl, List<MySprite>>(control, new List<MySprite>()));
            return this;
        }

        public void RemoveControl(IControl control)
        {
            var controlIndex = _content.FindIndex(x => x.Key == control);
            if (controlIndex > -1)
            {
                control.RedrawRequired -= OnRedrawRequired;
                control.Dispose();
                _content.RemoveAt(controlIndex);
            }
        }

        public void ClearControls()
        {
            foreach (var control in _content.Select(x => x.Key))
            {
                control.RedrawRequired -= OnRedrawRequired;
                control.Dispose();
            }

            _content.Clear();
        }

        public void Redraw(bool flush = false)
        {
            if (!flush && !_contentDirty)
                return;

            using (var frame = _surface.DrawFrame())
            {
                {   // Add background.
                    frame.Add(DrawRect(
                        position: _viewport.Position,
                        size: _surface.SurfaceSize,
                        color: _config.BaseBackgroundColor
                    ));

                    frame.Add(DrawRect(
                        position: _viewport.Position,
                        size: _surface.SurfaceSize,
                        color: _config.BaseBackgroundColor,
                        spriteName: Sprites.GridTexture
                    ));
                }

                // As a simplification, we're implementing automatic vertical stacking. +1 is to mitigate a visual artifact on the bottom of the screen when the top line is drawn onto.
                // TODO: Implement separate layout elements for vertical and horizontal stacking.
                var verticalWritingPosition = _viewport.Position.Y + 1;

                foreach (var entry in _content)
                {
                    var control = entry.Key;
                    if (control.Visibility == Visibility.NotRendered)
                        continue;

                    var spriteList = entry.Value;

                    // TODO: Add dirty flag to controls, and if not dirty, use cached.
                    verticalWritingPosition = DrawControl(control, spriteList, verticalWritingPosition);
                    frame.AddRange(spriteList);
                }
            }

            _contentDirty = false;
        }

        /// <summary>
        /// Generates sprites for the specified control in the specified list.
        /// </summary>
        /// <returns>Returns the resulting vertical writing position after the control is rendered.</returns>
        private float DrawControl(IControl control, List<MySprite> spriteList, float verticalWritingPosition)
        {
            spriteList.Clear();

            var viewportWidth = _viewport.Size.X;
            var viewportHeight = _viewport.Size.Y;

            var fontName = string.IsNullOrEmpty(control.FontName) ? _config.BaseFontName : control.FontName;
            var fontSize = control.FontSize * _config.BaseFontSize;
            var emSize = _surface.MeasureStringInPixels(_buffer.Clear().Append('X'), fontName, fontSize).Y;

            var paddingSize = CalculateThickness(control.Padding, control.PaddingUnit, emSize, viewportWidth, viewportHeight);
            var borderSize = CalculateThickness(control.Border, control.BorderUnit, emSize, viewportWidth, viewportHeight);
            var marginSize = CalculateThickness(control.Margin, control.MarginUnit, emSize, viewportWidth, viewportHeight);

            var controlTopLeft = new Vector2(0 + marginSize.Left, verticalWritingPosition + marginSize.Top);
            var contentTopLeft = new Vector2(controlTopLeft.X + borderSize.Left + paddingSize.Left, controlTopLeft.Y + borderSize.Top + paddingSize.Top);
            var controlWidth = CalculateSize(control.Width, control.WidthUnit, emSize, viewportWidth);

            var remainingLineCapacity = (int)((_viewport.Bottom - paddingSize.Bottom - borderSize.Bottom - contentTopLeft.Y) / emSize);
            var maxLineLength = controlWidth > 0
                ? controlWidth - paddingSize.Left - paddingSize.Right
                : viewportWidth - paddingSize.Left - paddingSize.Right;
            var content = control.GetContent(new ContentGenerationHelper(remainingLineCapacity, _wordWrapper.SetUp(maxLineLength, _surface, fontName, fontSize)));
            var contentSize = _surface.MeasureStringInPixels(content, fontName, fontSize);

            var paddedContentSize = paddingSize + contentSize;

            var controlSize = new Vector2()
            {
                X = Math.Max(controlWidth, paddedContentSize.X),
                Y = Math.Max(CalculateSize(control.Height, control.HeightUnit, emSize, viewportHeight), paddedContentSize.Y)
            };
            var borderedControlSize = borderSize + controlSize;

            if (controlSize.X > paddedContentSize.X)
            {
                if (control.HorizontalAlignment == HorizontalAlignment.Center)
                    contentTopLeft.X += ((controlSize.X - paddingSize.Right - paddingSize.Left) * 0.5f) - contentSize.X * 0.5f;
                else if (control.HorizontalAlignment == HorizontalAlignment.Right)
                    contentTopLeft.X = controlTopLeft.X + controlSize.X - paddingSize.Right - contentSize.X;
            }

            if (controlSize.Y > paddedContentSize.Y)
            {
                if (control.VerticalAlignment == VerticalAlignment.Middle)
                    contentTopLeft.Y += ((controlSize.Y - paddingSize.Top - paddingSize.Bottom) * 0.5f) - contentSize.Y * 0.5f;
                else if (control.VerticalAlignment == VerticalAlignment.Bottom)
                    contentTopLeft.Y = controlTopLeft.Y + controlSize.Y - paddingSize.Bottom - contentSize.Y;
            }

            if (control.Visibility == Visibility.Visible)
            {
                if (!borderSize.IsZero && control.BorderColor != null)
                {
                    DrawBorders(control.BorderColor.Value, borderSize, controlTopLeft, borderedControlSize, spriteList);
                }

                // Add background if color is different than base background color
                if (control.BackgroundColor != null && control.BackgroundColor != _config.BaseBackgroundColor)
                {
                    spriteList.Add(DrawRect(
                        size: borderedControlSize,
                        position: new Vector2(controlTopLeft.X, controlTopLeft.Y),
                        color: control.BackgroundColor.Value
                    ));
                }

                // Add text content
                spriteList.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = content.ToString(),
                    Color = control.TextColor ?? _config.BaseFontColor,
                    Alignment = TextAlignment.LEFT,
                    RotationOrScale = control.FontSize * _config.BaseFontSize,
                    FontId = fontName,
                    Position = contentTopLeft
                });
            }

            return verticalWritingPosition + borderedControlSize.Y + marginSize.Vertical;
        }

        public float CalculateSize(float baseValue, SizeUnit sizeUnit, float emSize, float viewportSize)
        {
            switch (sizeUnit)
            {
                case SizeUnit.Em:
                    return baseValue * emSize;
                case SizeUnit.Px:
                    return baseValue;
                case SizeUnit.Dip:
                    throw new Exception("Not implemented."); // TODO: implement.
                case SizeUnit.Percent:
                    return viewportSize * (baseValue / 100);
                default:
                    throw new Exception($"Unknown enum value '{baseValue}' encountered in enum '{baseValue.GetType().Name}'.");
            }
        }

        public Thickness CalculateThickness(Thickness baseValue, SizeUnit sizeUnit, float emSize, float viewportWidth, float viewportHeight)
        {
            switch (sizeUnit)
            {
                case SizeUnit.Em:
                    return baseValue * emSize;
                case SizeUnit.Px:
                    return baseValue;
                case SizeUnit.Dip:
                    throw new Exception("Not implemented."); // TODO: implement.
                case SizeUnit.Percent:
                    return new Thickness()
                    {
                        Left = viewportWidth * (baseValue.Left / 100),
                        Right = viewportWidth * (baseValue.Right / 100),
                        Top = viewportHeight * (baseValue.Top / 100),
                        Bottom = viewportHeight * (baseValue.Bottom/ 100),
                    };
                default:
                    throw new Exception($"Unknown enum value '{baseValue}' encountered in enum '{baseValue.GetType().Name}'.");
            }
        }

        private void DrawBorders(Color borderColor, Thickness borderSize, Vector2 topLeftCorner, Vector2 rectangleSize, List<MySprite> spriteList)
        {
            // Correction to avoid overlapping border sprites on corners.
            var horizontalBorderWidth = rectangleSize.X - borderSize.Left - borderSize.Right;

            if (borderSize.Left > 0)
                spriteList.Add(DrawRect(
                    position: new Vector2(topLeftCorner.X, topLeftCorner.Y),
                    size: new Vector2(borderSize.Left, rectangleSize.Y),
                    color: borderColor));

            if (borderSize.Right > 0)
                spriteList.Add(DrawRect(
                    position: new Vector2(topLeftCorner.X + rectangleSize.X - borderSize.Right, topLeftCorner.Y),
                    size: new Vector2(borderSize.Right, rectangleSize.Y),
                    color: borderColor));

            if (borderSize.Top > 0)
                spriteList.Add(DrawRect(
                    position: new Vector2(topLeftCorner.X + borderSize.Left, topLeftCorner.Y),
                    size: new Vector2(horizontalBorderWidth, borderSize.Top),
                    color: borderColor));

            if (borderSize.Bottom > 0)
                spriteList.Add(DrawRect(
                    position: new Vector2(topLeftCorner.X + borderSize.Left, topLeftCorner.Y + rectangleSize.Y - borderSize.Bottom),
                    size: new Vector2(horizontalBorderWidth, borderSize.Bottom),
                    color: borderColor));
        }

        private MySprite DrawRect(Vector2 position, Vector2 size, Color color, string spriteName = Sprites.Rectangle)
        {
            position.Y += size.Y * 0.5f;

            return new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = spriteName,
                Alignment = TextAlignment.LEFT,
                Color = color,
                Position = position,
                Size = size
            };
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
            _config.BaseLineHeight = _surface.MeasureStringInPixels(_buffer.Clear().Append("X"), _config.BaseFontName, _config.BaseFontSize).Y;
            _config.BaseLineSpacing = _surface.MeasureStringInPixels(_buffer.Clear().Append("X\r\nX"), _config.BaseFontName, _config.BaseFontSize).Y - (2 * _config.BaseLineHeight);

            _config.PaddingLeft = 1;
        }

        private void SetupSurface()
        {
            _surface.ContentType = ContentType.SCRIPT;
            _surface.Script = "";
            _surface.PreserveAspectRatio = true;
        }

        private void DecommissionSurface()
        {
            _surface.ContentType = ContentType.TEXT_AND_IMAGE;
            _surface.WriteText(string.Empty);
        }

        private void OnRedrawRequired(IControl control)
        {
            _contentDirty = true;
        }

        public void SetFontType(string fontName)
        {
            _config.BaseFontName = fontName;
            SetupSurface();
            AdaptToSurface();
            Redraw(flush: true);
        }

        public void SetFontSize(float fontSize)
        {
            if (fontSize < 0 || fontSize > 10)
                throw new Exception("Font size must be between 0 and 10.");

            _config.BaseFontSize = fontSize;
            SetupSurface();
            AdaptToSurface();
            Redraw(flush: true);
        }

        public void SetFontColor(Color color)
        {
            _config.BaseFontColor = color;
            SetupSurface();
            Redraw(flush: true);
        }

        public void SetBackgroundColor(Color color)
        {
            _config.BaseBackgroundColor = color;
            SetupSurface();
            Redraw(flush: true);
        }
    }
}
