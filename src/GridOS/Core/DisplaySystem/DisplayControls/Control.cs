using System;
using System.Text;

namespace IngameScript
{
    /// <summary>
    /// Represents a simple control that defines a rectangle with optional text content.
    /// </summary>
    abstract class Control : IControl
    {
        public event Action<IControl> RedrawRequired;

        public Visibility Visibility
        {
            get { return visibility; }
            set { visibility = value; OnRedrawRequired(); }
        }
        private Visibility visibility;

        /// <summary>
        /// Specifies how to apply the offset of the control.
        /// </summary>
        public Positioning Positioning { get; set; }

        public float Width { get; set; }
        public SizeUnit WidthUnit { get; set; }
        public float Height { get; set; }
        public SizeUnit HeightUnit { get; set; }

        public Thickness Margin { get; set; }
        public SizeUnit MarginUnit { get; set; }
        public Thickness Padding { get; set; }
        public SizeUnit PaddingUnit { get; set; }

        public SizeUnit BorderUnit { get; set; }
        public Thickness Border { get; set; }
        public Color? BorderColor { get; set; }

        public string FontName { get; set; } = null;
        public float FontSize { get; set; } = 1f;

        public Color? TextColor { get; set; }
        public Color? BackgroundColor { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }

        public abstract StringBuilder GetContent(ContentGenerationHelper contentHelper, bool FlushCache = false);

        protected virtual void OnRedrawRequired()
        {
            RedrawRequired?.Invoke(this);
        }

        public abstract void Dispose();
    }
}
