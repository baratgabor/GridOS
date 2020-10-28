using System;

namespace IngameScript
{
    public struct MySprite : IEquatable<MySprite>
    {
        //
        // Summary:
        //     Type of the render layer
        public SpriteType Type;
        //
        // Summary:
        //     Render position for this layer. If not set, it will be placed in the center
        public Vector2? Position;
        //
        // Summary:
        //     Render size for this layer. If not set, it will be sized to take up the whole
        //     texture
        public Vector2? Size;
        //
        // Summary:
        //     Color mask to be used when rendering this layer. If not set, white will be used
        public Color? Color;
        //
        // Summary:
        //     Data to be rendered, depending on what the layer type is. This can be text or
        //     a texture path
        public string Data;
        //
        // Summary:
        //     In case we are rendering text, what font to use.
        public string FontId;
        //
        // Summary:
        //     Alignment for the text and sprites.
        public TextAlignment Alignment;

        //
        // Summary:
        //     Rotation of sprite in radians. Used as scale for text.
        public float RotationOrScale;

        public MySprite(SpriteType type = SpriteType.TEXTURE, string data = null, Vector2? position = null, Vector2? size = null, Color? color = null, string fontId = null, TextAlignment alignment = TextAlignment.CENTER, float rotation = 0)
        {
            Type = type;
            Data = data;
            Position = position;
            Size = size;
            Color = color;
            FontId = fontId;
            Alignment = alignment;
            RotationOrScale = rotation;
        }

        public static MySprite CreateClearClipRect() => new MySprite();
        //public static MySprite CreateClipRect(Rectangle rect) => new MySprite();
        public static MySprite CreateSprite(string sprite, Vector2 position, Vector2 size) => new MySprite();
        public static MySprite CreateText(string text, string fontId, Color color, float scale = 1, TextAlignment alignment = TextAlignment.CENTER) => new MySprite();
        
        public bool Equals(MySprite other) => true;
        //public override bool Equals(object obj) => true;
        //public override int GetHashCode() => 1;

        //public static implicit operator MySerializableSprite(MySprite sprite);
        //public static implicit operator MySprite(MySerializableSprite sprite);
    }

    public enum SpriteType : byte
    {
        TEXTURE = 0,
        TEXT = 2,
        CLIP_RECT = 4
    }
}
