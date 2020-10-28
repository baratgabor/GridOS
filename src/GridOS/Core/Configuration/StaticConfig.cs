using System.Collections.Generic;

namespace IngameScript
{
    public static class StaticConfig
    {
        public static IReadOnlyList<float> FontSizes = new List<float>() { 0.5f, 0.75f, 1f, 1.25f, 1.5f };

        public static IReadOnlyList<ColorSetting> FontColors = new List<ColorSetting>()
        {
            new ColorSetting() { Name = "Cyan", Color = Color.Multiply(Color.Cyan, 0.9f) },
            new ColorSetting() { Name = "White", Color = Color.Multiply(Color.White, 0.9f) },
            new ColorSetting() { Name = "Yellow", Color = Color.Multiply(Color.Yellow, 0.9f) },
            new ColorSetting() { Name = "Black", Color = Color.Black },
            new ColorSetting() { Name = "Crimson", Color = Color.Multiply(Color.Crimson, 0.9f) },
            new ColorSetting() { Name = "Navy", Color = Color.Multiply(Color.Navy, 0.9f) },
            new ColorSetting() { Name = "Orchid", Color = Color.Multiply(Color.DarkOrchid, 0.9f) },
        };

        public static IReadOnlyList<ColorSetting> BackgroundColors = new List<ColorSetting>()
        {
            new ColorSetting() { Name = "Black", Color = Color.Black },
            new ColorSetting() { Name = "Cyan", Color = Color.Darken(Color.Cyan, 0.4f) },
            new ColorSetting() { Name = "Crimson", Color = Color.Darken(Color.Crimson, 0.4f) },
            new ColorSetting() { Name = "Navy", Color = Color.Darken(Color.Navy, 0.4f) },
            new ColorSetting() { Name = "Orchid", Color = Color.Darken(Color.DarkOrchid, 0.4f) },
        };

        public struct ColorSetting
        {
            public string Name;
            public Color Color;
        }
    }
}
