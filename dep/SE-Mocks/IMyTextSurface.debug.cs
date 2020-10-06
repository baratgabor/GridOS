using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    public interface IMyTextSurface
    {
        //
        // Summary:
        //     Gets or sets font size
        float FontSize { get; set; }
        //
        // Summary:
        //     Foreground color used for scripts.
        //Color ScriptForegroundColor { get; set; }
        //
        // Summary:
        //     Background color used for scripts.
        //Color ScriptBackgroundColor { get; set; }
        //
        // Summary:
        //     Text padding from all sides of the panel.
        float TextPadding { get; set; }
        //
        // Summary:
        //     Preserve aspect ratio of images.
        bool PreserveAspectRatio { get; set; }
        //
        // Summary:
        //     Size of the texture the drawing surface is rendered to.
        //Vector2 TextureSize { get; }
        //
        // Summary:
        //     Size of the drawing surface.
        //Vector2 SurfaceSize { get; }
        //
        // Summary:
        //     Type of content to be displayed on the screen.
        ContentType ContentType { get; set; }
        //
        // Summary:
        //     Currently running script
        string Script { get; set; }
        //
        // Summary:
        //     How should the text be aligned
        //TextAlignment Alignment { get; set; }
        //
        // Summary:
        //     Gets or sets the font
        string Font { get; set; }
        //
        // Summary:
        //     Gets or sets the change interval for selected textures
        float ChangeInterval { get; set; }
        //
        // Summary:
        //     Value for offscreen texture alpha channel - for PBR material it is metalness
        //     (should be 0) - for transparent texture it is opacity
        byte BackgroundAlpha { get; set; }
        //
        // Summary:
        //     Gets or sets background color
        //Color BackgroundColor { get; set; }
        //
        // Summary:
        //     Gets or sets font color
        //Color FontColor { get; set; }
        //
        // Summary:
        //     Identifier name of this surface.
        string Name { get; }
        //
        // Summary:
        //     Localized name of this surface.
        string DisplayName { get; }
        //
        // Summary:
        //     The image that is currently shown on the screen. Returns NULL if there are no
        //     images selected OR the screen is in text mode.
        string CurrentlyShownImage { get; }

        void AddImagesToSelection(List<string> ids, bool checkExistence = false);
        void AddImageToSelection(string id, bool checkExistence = false);
        void ClearImagesFromSelection();
        //
        // Summary:
        //     Creates a new draw frame where you can add sprites to be rendered.
        //MySpriteDrawFrame DrawFrame();
        //
        // Summary:
        //     Gets a list of available fonts
        //
        // Parameters:
        //   fonts:
        void GetFonts(List<string> fonts);
        //
        // Summary:
        //     Gets a list of available scripts
        //
        // Parameters:
        //   scripts:
        void GetScripts(List<string> scripts);
        //
        // Summary:
        //     Outputs the selected image ids to the specified list. NOTE: List is not cleared
        //     internally.
        //
        // Parameters:
        //   output:
        void GetSelectedImages(List<string> output);
        //
        // Summary:
        //     Gets a list of available sprites
        void GetSprites(List<string> sprites);
        string GetText();
        //
        // Summary:
        //     Calculates how many pixels a string of a given font and scale will take up.
        //
        // Parameters:
        //   text:
        //
        //   font:
        //
        //   scale:
        //Vector2 MeasureStringInPixels(StringBuilder text, string font, float scale);
        void ReadText(StringBuilder buffer, bool append = false);
        void RemoveImageFromSelection(string id, bool removeDuplicates = false);
        void RemoveImagesFromSelection(List<string> ids, bool removeDuplicates = false);
        bool WriteText(StringBuilder value, bool append = false);
        bool WriteText(string value, bool append = false);
    }

    public enum ContentType : byte
    {
        NONE = 0,
        TEXT_AND_IMAGE = 1,
        IMAGE = 2,
        SCRIPT = 3
    }
}
