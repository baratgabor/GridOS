using System.Text;
using System;

namespace IngameScript
{
    interface IControl
    {
        bool Visible { get; set; }

        /// <summary>
        /// Specifies how to apply the offset of the control.
        /// </summary>
        Positioning Positioning { get; set; }

        /// <summary>
        /// Offsets the control from its position, depending on the positioning specified.
        /// </summary>
        Vector2 Offset { get; set; }

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
        Color BorderColor { get; set; }

        string FontName { get; set; }
        float FontSize { get; set; }

        Color? TextColor { get; set; }
        Color? BackgroundColor { get; set; }

        TextAlignment TextAlignment { get; set; }

        event Action<IControl> RedrawRequired;

        StringBuilder GetContent(bool FlushCache = false);
    }
}
