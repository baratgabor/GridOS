using System.Collections.Generic;

namespace IngameScript
{
    public static class StaticConfig
    {
        public static class BackgroundColors
        {
            public static Color Black = Color.Black;
            public static Color Cyan = Color.Darken(Color.Cyan, 0.7f);
            public static Color Crimson = Color.Darken(Color.Crimson, 0.5f);
            public static Color Navy = Color.Darken(Color.Navy, 0.5f);
            public static Color Orchid = Color.Darken(Color.DarkOrchid, 0.7f);
            public static Color Keen1 = new Color(43, 54, 61);
            public static Color Keen2 = new Color(85, 109, 117);

            public static IReadOnlyList<NamedOption<Color>> AsList2 = new List<NamedOption<Color>>()
            {
                new NamedOption<Color>() { Name = nameof(Black), Value = Black },
                new NamedOption<Color>() { Name = nameof(Cyan), Value = Cyan },
                new NamedOption<Color>() { Name = nameof(Crimson), Value = Crimson },
                new NamedOption<Color>() { Name = nameof(Navy), Value = Navy },
                new NamedOption<Color>() { Name = nameof(Orchid), Value = Orchid },
                new NamedOption<Color>() { Name = nameof(Keen1), Value = Keen1 },
                new NamedOption<Color>() { Name = nameof(Keen2), Value = Keen2 },
            };
        }

        public static class FontColors
        {
            public static Color Cyan = Color.Cyan;
            public static Color White = Color.Multiply(Color.White, 0.9f);
            public static Color Yellow = Color.Multiply(Color.Yellow, 0.9f);
            public static Color Black = Color.Black;
            public static Color Crimson = Color.Crimson;
            public static Color Navy = Color.Navy;
            public static Color Orchid = Color.DarkOrchid;
            public static Color Wheat = Color.Wheat;
            public static Color Keen1 = new Color(186, 238, 249);
            public static Color Keen2 = new Color(198, 218, 226);

            public static IReadOnlyList<NamedOption<Color>> AsList2 = new List<NamedOption<Color>>()
            {
                new NamedOption<Color>() { Name = nameof(Cyan), Value = Cyan },
                new NamedOption<Color>() { Name = nameof(White), Value = White },
                new NamedOption<Color>() { Name = nameof(Yellow), Value = Yellow },
                new NamedOption<Color>() { Name = nameof(Black), Value = Black },
                new NamedOption<Color>() { Name = nameof(Crimson), Value = Crimson },
                new NamedOption<Color>() { Name = nameof(Navy), Value = Navy },
                new NamedOption<Color>() { Name = nameof(Orchid), Value = Orchid },
                new NamedOption<Color>() { Name = nameof(Wheat), Value = Wheat},
                new NamedOption<Color>() { Name = nameof(Keen1), Value = Keen1 },
                new NamedOption<Color>() { Name = nameof(Keen2), Value = Keen2 },
            };
        }

        public static class FontSizes
        {
            public static float Percent50 = 0.5f;
            public static float Percent75 = 0.75f;
            public static float Percent100 = 1f;
            public static float Percent125 = 1.25f;
            public static float Percent150 = 1.5f;

            public static IReadOnlyList<NamedOption<float>> AsList = new List<NamedOption<float>>()
            {
                new NamedOption<float>("50%", Percent50),
                new NamedOption<float>("75%", Percent75),
                new NamedOption<float>("100%", Percent100),
                new NamedOption<float>("125%", Percent125),
                new NamedOption<float>("150%", Percent150)
            };
        }

        public static class FontTypes
        {
            public static string Default = "White";
            public static string Outlined = "Debug";
            public static string Monospace = "Monospace";

            public static IReadOnlyList<NamedOption<string>> AsList = new List<NamedOption<string>>()
            {
                new NamedOption<string>(nameof(Default), Default),
                new NamedOption<string>(nameof(Outlined), Outlined),
                new NamedOption<string>(nameof(Monospace), Monospace)
            };
        }
    }
}
