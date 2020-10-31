using System.Collections.Generic;

namespace IngameScript
{
    public static class StaticConfig
    {
        public static IReadOnlyList<float> FontSizes = new List<float>() { 0.5f, 0.75f, 1f, 1.25f, 1.5f };

        public static IReadOnlyList<ColorSetting> FontColors = new List<ColorSetting>()
        {
            new ColorSetting() { Name = "Cyan", Color = Color.Cyan },
            new ColorSetting() { Name = "White", Color = Color.Multiply(Color.White, 0.9f) },
            new ColorSetting() { Name = "Yellow", Color = Color.Multiply(Color.Yellow, 0.9f) },
            new ColorSetting() { Name = "Black", Color = Color.Black },
            new ColorSetting() { Name = "Crimson", Color = Color.Crimson },
            new ColorSetting() { Name = "Navy", Color = Color.Navy },
            new ColorSetting() { Name = "Orchid", Color = Color.DarkOrchid },
            new ColorSetting() { Name = "Wheat", Color = Color.Wheat},
            new ColorSetting() { Name = "Keen 1", Color = new Color(186, 238, 249) },
            new ColorSetting() { Name = "Keen 2", Color = new Color(198, 218, 226) },
        };

        public static IReadOnlyList<ColorSetting> BackgroundColors = new List<ColorSetting>()
        {
            new ColorSetting() { Name = "Black", Color = Color.Black },
            new ColorSetting() { Name = "Cyan", Color = Color.Darken(Color.Cyan, 0.7f) },
            new ColorSetting() { Name = "Crimson", Color = Color.Darken(Color.Crimson, 0.5f) },
            new ColorSetting() { Name = "Navy", Color = Color.Darken(Color.Navy, 0.4f) },
            new ColorSetting() { Name = "Orchid", Color = Color.Darken(Color.DarkOrchid, 0.5f) },
            new ColorSetting() { Name = "Keen 1", Color = new Color(43, 54, 61) },
            new ColorSetting() { Name = "Keen 2", Color = new Color(85, 109, 117) },
        };

        public struct ColorSetting
        {
            public string Name;
            public Color Color;
        }
    }
}
