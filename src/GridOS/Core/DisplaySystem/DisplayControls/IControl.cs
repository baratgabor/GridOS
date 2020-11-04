using System.Text;
using System;

namespace IngameScript
{
    interface IControl : IDisposable
    {
        Visibility Visibility { get; set; }

        /// <summary>
        /// Specifies how to apply the offset of the control.
        /// </summary>
        Positioning Positioning { get; set; }

        Thickness Margin { get; set; }
        SizeUnit MarginUnit { get; set; }
        Thickness Padding { get; set; }
        SizeUnit PaddingUnit { get; set; }

        float Width { get; set; }
        SizeUnit WidthUnit { get; set; }
        float Height { get; set; }
        SizeUnit HeightUnit { get; set; }

        Thickness Border { get; set; }
        SizeUnit BorderUnit { get; set; }
        Color? BorderColor { get; set; }

        string FontName { get; set; }
        float FontSize { get; set; }

        Color? TextColor { get; set; }
        Color? BackgroundColor { get; set; }

        /// <summary>
        /// Specifies vertical alignment of content when control size exceeds content size.
        /// </summary>
        VerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// Specifies horizontal alignment of content when control size exceeds content size.
        /// </summary>
        HorizontalAlignment HorizontalAlignment { get; set; }

        event Action<IControl> RedrawRequired;

        StringBuilder GetContent(ContentGenerationHelper contentHelper, bool FlushCache = false);
    }
}
