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
        protected IMyTextSurface _surface;

        protected bool _contentDirty = true;

        protected List<KeyValuePair<IControl, List<MySprite>>> _content = new List<KeyValuePair<IControl, List<MySprite>>>();

        protected MainConfig _config;

        protected RectangleF _viewport;

        private StringBuilder _buffer = new StringBuilder();

        public DisplayView(IMyTextSurface surface, MainConfig config)
        {
            _surface = surface;
            _config = config;

            SetupSurface();
            AdaptToSurface();
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
                _content.RemoveAt(controlIndex);
            }
        }

        public void ClearControls()
        {
            foreach (var kvp in _content)
                kvp.Key.RedrawRequired -= OnRedrawRequired;

            _content.Clear();
        }

        public void Redraw(bool flush = false)
        {
            if (!flush && !_contentDirty)
                return;

            using (var frame = _surface.DrawFrame())
            {
                {   // Add background.
                    frame.Add(new MySprite()
                    {
                        Type = SpriteType.TEXTURE,
                        Data = "SquareSimple",
                        Color = _config.BaseBackgroundColor,
                        Alignment = TextAlignment.CENTER,
                        Position = _viewport.Center,
                        Size = _surface.SurfaceSize
                    });

                    frame.Add(new MySprite()
                    {
                        Type = SpriteType.TEXTURE,
                        Data = "Grid",
                        Color = new Color(0, 0, 0, 170),
                        Alignment = TextAlignment.CENTER,
                        Position = _viewport.Center,
                        Size = _surface.SurfaceSize
                    });
                }

                // As a simplification, we're implementing automatic vertical stacking. +1 is to mitigate a visual artifact on the bottom of the screen when the top line is drawn onto.
                // TODO: Implement separate layout elements for vertical and horizontal stacking.
                var verticalWritingPosition = _viewport.Position.Y + 1;

                for (int i = 0; i < _content.Count; i++)
                {
                    var controlEntry = _content[i];
                    if (controlEntry.Key.Visible)
                    {
                        // TODO: Add dirty flag to controls, and if not dirty, use cached.
                        verticalWritingPosition = GenerateSpritesForControl(controlEntry.Key, controlEntry.Value, verticalWritingPosition);
                        frame.AddRange(controlEntry.Value);
                    }
                }
            }

            _contentDirty = false;
        }

        /// <summary>
        /// Generates sprites for the specified control in the specified list.
        /// </summary>
        /// <returns>Returns the resulting vertical writing position after the control is rendered.</returns>
        private float GenerateSpritesForControl(IControl control, List<MySprite> targetList, float verticalWritingPosition)
        {
            targetList.Clear();

            var fontName = string.IsNullOrEmpty(control.FontName) ? _config.BaseFontName : control.FontName;
            var fontSize = control.FontSize * _config.BaseFontSize;
            var emSize = _surface.MeasureStringInPixels(_buffer.Clear().Append('X'), fontName, fontSize).Y;

            var paddingSize = CalculateThickness(control.Padding, control.PaddingUnit, emSize, _viewport.Size.X, _viewport.Size.Y);

            var controlTopLeft = new Vector2(0, verticalWritingPosition) + control.Offset;
            var contentTopLeft = new Vector2(controlTopLeft.X + paddingSize.Left, controlTopLeft.Y + paddingSize.Top);

            var remainingLineCapacity = (int)((_viewport.Bottom - paddingSize.Bottom - contentTopLeft.Y) / emSize);
            var content = control.GetContent(remainingLineCapacity);
            var contentSize = _surface.MeasureStringInPixels(content, fontName, fontSize);

            var paddedContentSize = paddingSize + contentSize;

            var controlSize = new Vector2()
            {
                X = Math.Max(CalculateSize(control.Width, control.WidthUnit, emSize, _viewport.Size.X), paddedContentSize.X),
                Y = Math.Max(CalculateSize(control.Height, control.HeightUnit, emSize, _viewport.Size.Y), paddedContentSize.Y)
            };

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

            // Add background if color is different than base background color
            if (control.BackgroundColor != null && control.BackgroundColor != _config.BaseBackgroundColor)
            {
                targetList.Add(new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Color = control.BackgroundColor,
                    Alignment = TextAlignment.LEFT,
                    Size = controlSize,
                    Position = new Vector2(controlTopLeft.X, controlTopLeft.Y + (controlSize.Y / 2))
                });
            }

            // Add text content
            targetList.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = content.ToString(),
                Color = control.TextColor ?? _config.BaseFontColor,
                Alignment = TextAlignment.LEFT,
                RotationOrScale = control.FontSize * _config.BaseFontSize,
                FontId = fontName,
                Position = contentTopLeft
            });

            return verticalWritingPosition + controlSize.Y;
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
                        Right = viewportWidth / (baseValue.Right / 100),
                        Top = viewportHeight * (baseValue.Top / 100),
                        Bottom = viewportHeight / (baseValue.Bottom/ 100),
                    };
                default:
                    throw new Exception($"Unknown enum value '{baseValue}' encountered in enum '{baseValue.GetType().Name}'.");
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
