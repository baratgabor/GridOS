using System.Collections.Generic;
using System.Runtime.Hosting;
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
        Color ScriptForegroundColor { get; set; }
        //
        // Summary:
        //     Background color used for scripts.
        Color ScriptBackgroundColor { get; set; }
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
        Vector2 TextureSize { get; }
        //
        // Summary:
        //     Size of the drawing surface.
        Vector2 SurfaceSize { get; }
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
        TextAlignment Alignment { get; set; }
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
        Color BackgroundColor { get; set; }
        //
        // Summary:
        //     Gets or sets font color
        Color FontColor { get; set; }
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
        MySpriteDrawFrame DrawFrame();
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
        Vector2 MeasureStringInPixels(StringBuilder text, string font, float scale);
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

    public enum TextAlignment : byte
    {
        LEFT = 0,
        RIGHT = 1,
        CENTER = 2
    }

    public struct Color
    {
        //
        // Summary:
        //     Gets a system-defined color with the value R:60 G:179 B:113 A:255.
        public static Color MediumSeaGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:0 B:255 A:255.
        public static Color Magenta { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:128 G:0 B:0 A:255.
        public static Color Maroon { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:102 G:205 B:170 A:255.
        public static Color MediumAquamarine { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:0 B:205 A:255.
        public static Color MediumBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:186 G:85 B:211 A:255.
        public static Color MediumOrchid { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:147 G:112 B:219 A:255.
        public static Color MediumPurple { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:123 G:104 B:238 A:255.
        public static Color MediumSlateBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:245 G:255 B:250 A:255.
        public static Color MintCream { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:72 G:209 B:204 A:255.
        public static Color MediumTurquoise { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:199 G:21 B:133 A:255.
        public static Color MediumVioletRed { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:25 G:25 B:112 A:255.
        public static Color MidnightBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:250 G:240 B:230 A:255.
        public static Color Linen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:228 B:225 A:255.
        public static Color MistyRose { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:228 B:181 A:255.
        public static Color Moccasin { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:222 B:173 A:255.
        public static Color NavajoWhite { get; }
        //
        // Summary:
        //     Gets a system-defined color R:0 G:0 B:128 A:255.
        public static Color Navy { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:250 B:154 A:255.
        public static Color MediumSpringGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:50 G:205 B:50 A:255.
        public static Color LimeGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:135 G:206 B:250 A:255.
        public static Color LightSkyBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:255 B:224 A:255.
        public static Color LightYellow { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:255 B:240 A:255.
        public static Color Ivory { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:240 G:230 B:140 A:255.
        public static Color Khaki { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:230 G:230 B:250 A:255.
        public static Color Lavender { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:240 B:245 A:255.
        public static Color LavenderBlush { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:124 G:252 B:0 A:255.
        public static Color LawnGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:250 B:205 A:255.
        public static Color LemonChiffon { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:173 G:216 B:230 A:255.
        public static Color LightBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:240 G:128 B:128 A:255.
        public static Color LightCoral { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:255 B:0 A:255.
        public static Color Lime { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:224 G:255 B:255 A:255.
        public static Color LightCyan { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:144 G:238 B:144 A:255.
        public static Color LightGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:211 G:211 B:211 A:255.
        public static Color LightGray { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:182 B:193 A:255.
        public static Color LightPink { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:160 B:122 A:255.
        public static Color LightSalmon { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:32 G:178 B:170 A:255.
        public static Color LightSeaGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:253 G:245 B:230 A:255.
        public static Color OldLace { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:119 G:136 B:153 A:255.
        public static Color LightSlateGray { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:176 G:196 B:222 A:255.
        public static Color LightSteelBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:250 G:250 B:210 A:255.
        public static Color LightGoldenrodYellow { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:128 G:128 B:0 A:255.
        public static Color Olive { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:238 G:232 B:170 A:255.
        public static Color PaleGoldenrod { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:165 B:0 A:255.
        public static Color Orange { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:192 G:192 B:192 A:255.
        public static Color Silver { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:135 G:206 B:235 A:255.
        public static Color SkyBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:106 G:90 B:205 A:255.
        public static Color SlateBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:112 G:128 B:144 A:255.
        public static Color SlateGray { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:250 B:250 A:255.
        public static Color Snow { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:255 B:127 A:255.
        public static Color SpringGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:70 G:130 B:180 A:255.
        public static Color SteelBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:210 G:180 B:140 A:255.
        public static Color Tan { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:160 G:82 B:45 A:255.
        public static Color Sienna { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:128 B:128 A:255.
        public static Color Teal { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:99 B:71 A:255.
        public static Color Tomato { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:64 G:224 B:208 A:255.
        public static Color Turquoise { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:238 G:130 B:238 A:255.
        public static Color Violet { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:245 G:222 B:179 A:255.
        public static Color Wheat { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:255 B:255 A:255.
        public static Color White { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:245 G:245 B:245 A:255.
        public static Color WhiteSmoke { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:255 B:0 A:255.
        public static Color Yellow { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:154 G:205 B:50 A:255.
        public static Color YellowGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:216 G:191 B:216 A:255.
        public static Color Thistle { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:107 G:142 B:35 A:255.
        public static Color OliveDrab { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:245 B:238 A:255.
        public static Color SeaShell { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:244 G:164 B:96 A:255.
        public static Color SandyBrown { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:69 B:0 A:255.
        public static Color OrangeRed { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:218 G:112 B:214 A:255.
        public static Color Orchid { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:75 G:0 B:130 A:255.
        public static Color Indigo { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:152 G:251 B:152 A:255.
        public static Color PaleGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:175 G:238 B:238 A:255.
        public static Color PaleTurquoise { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:219 G:112 B:147 A:255.
        public static Color PaleVioletRed { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:239 B:213 A:255.
        public static Color PapayaWhip { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:218 B:185 A:255.
        public static Color PeachPuff { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:46 G:139 B:87 A:255.
        public static Color SeaGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:205 G:133 B:63 A:255.
        public static Color Peru { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:221 G:160 B:221 A:255.
        public static Color Plum { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:176 G:224 B:230 A:255.
        public static Color PowderBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:128 G:0 B:128 A:255.
        public static Color Purple { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:0 B:0 A:255.
        public static Color Red { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:188 G:143 B:143 A:255.
        public static Color RosyBrown { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:65 G:105 B:225 A:255.
        public static Color RoyalBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:139 G:69 B:19 A:255.
        public static Color SaddleBrown { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:250 G:128 B:114 A:255.
        public static Color Salmon { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:192 B:203 A:255.
        public static Color Pink { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:205 G:92 B:92 A:255.
        public static Color IndianRed { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:105 B:180 A:255.
        public static Color HotPink { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:240 G:255 B:240 A:255.
        public static Color Honeydew { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:184 G:134 B:11 A:255.
        public static Color DarkGoldenrod { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:139 B:139 A:255.
        public static Color DarkCyan { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:0 B:139 A:255.
        public static Color DarkBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:255 B:255 A:255.
        public static Color Cyan { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:220 G:20 B:60 A:255.
        public static Color Crimson { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:248 B:220 A:255.
        public static Color Cornsilk { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:100 G:149 B:237 A:255.
        public static Color CornflowerBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:127 B:80 A:255.
        public static Color Coral { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:210 G:105 B:30 A:255.
        public static Color Chocolate { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:127 G:255 B:0 A:255.
        public static Color Chartreuse { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:95 G:158 B:160 A:255.
        public static Color CadetBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:169 G:169 B:169 A:255.
        public static Color DarkGray { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:222 G:184 B:135 A:255.
        public static Color BurlyWood { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:138 G:43 B:226 A:255.
        public static Color BlueViolet { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:0 B:255 A:255.
        public static Color Blue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:235 B:205 A:255.
        public static Color BlanchedAlmond { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:228 B:196 A:255.
        public static Color Bisque { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:245 G:245 B:220 A:255.
        public static Color Beige { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:240 G:255 B:255 A:255.
        public static Color Azure { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:127 G:255 B:212 A:255.
        public static Color Aquamarine { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:255 B:255 A:255.
        public static Color Aqua { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:250 G:235 B:215 A:255.
        public static Color AntiqueWhite { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:240 G:248 B:255 A:255.
        public static Color AliceBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:0 B:0 A:0.
        public static Color Transparent { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:165 G:42 B:42 A:255.
        public static Color Brown { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:100 B:0 A:255.
        public static Color DarkGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:0 B:0 A:255.
        public static Color Black { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:139 G:0 B:139 A:255.
        public static Color DarkMagenta { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:189 G:183 B:107 A:255.
        public static Color DarkKhaki { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:173 G:255 B:47 A:255.
        public static Color GreenYellow { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:128 B:0 A:255.
        public static Color Green { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:128 G:128 B:128 A:255.
        public static Color Gray { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:218 G:165 B:32 A:255.
        public static Color Goldenrod { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:215 B:0 A:255.
        public static Color Gold { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:248 G:248 B:255 A:255.
        public static Color GhostWhite { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:220 G:220 B:220 A:255.
        public static Color Gainsboro { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:0 B:255 A:255.
        public static Color Fuchsia { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:34 G:139 B:34 A:255.
        public static Color ForestGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:250 B:240 A:255.
        public static Color FloralWhite { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:178 G:34 B:34 A:255.
        public static Color Firebrick { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:30 G:144 B:255 A:255.
        public static Color DodgerBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:191 B:255 A:255.
        public static Color DeepSkyBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:20 B:147 A:255.
        public static Color DeepPink { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:148 G:0 B:211 A:255.
        public static Color DarkViolet { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:0 G:206 B:209 A:255.
        public static Color DarkTurquoise { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:47 G:79 B:79 A:255.
        public static Color DarkSlateGray { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:72 G:61 B:139 A:255.
        public static Color DarkSlateBlue { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:143 G:188 B:139 A:255.
        public static Color DarkSeaGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:233 G:150 B:122 A:255.
        public static Color DarkSalmon { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:139 G:0 B:0 A:255.
        public static Color DarkRed { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:153 G:50 B:204 A:255.
        public static Color DarkOrchid { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:255 G:140 B:0 A:255.
        public static Color DarkOrange { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:85 G:107 B:47 A:255.
        public static Color DarkOliveGreen { get; }
        //
        // Summary:
        //     Gets a system-defined color with the value R:105 G:105 B:105 A:255.
        public static Color DimGray { get; }
        //
        // Summary:
        //     Gets or sets the blue component value of this Color.
        public byte Z { get; set; }
        //
        // Summary:
        //     Gets or sets the red component value of this Color.
        public byte X { get; set; }
        //
        // Summary:
        //     Gets or sets the red component value of this Color.
        public byte R { get; set; }
        //
        // Summary:
        //     Gets or sets the green component value of this Color.
        public byte Y { get; set; }
        //
        // Summary:
        //     Gets or sets the blue component value of this Color.
        public byte B { get; set; }
        //
        // Summary:
        //     Gets or sets the alpha component value.
        public byte A { get; set; }
        //
        // Summary:
        //     Gets or sets the green component value of this Color.
        public byte G { get; set; }

        public static Color Lighten(Color inColor, double inAmount)
        {
            return inColor;
        }

        public static Color Darken(Color inColor, double inAmount)
        {
            return inColor;
        }

        public static Color Multiply(Color value, float scale)
        {
            return new Color()
            {
                R = (byte)(value.R * scale),
                G = (byte)(value.G * scale),
                B = (byte)(value.B * scale)
            };
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
        }

        public static bool operator !=(Color a, Color b)
        {
            return !(a == b);
        }
    }
}
