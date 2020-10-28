namespace IngameScript
{
    public struct MySpriteCollection
    {
        public MySprite[] Sprites { get; private set; }

        public MySpriteCollection(MySprite[] sprites) { Sprites = sprites; }

        //public static implicit operator MySerializableSpriteCollection(MySpriteCollection collection) { }
        //public static implicit operator MySpriteCollection(MySerializableSpriteCollection collection) { }

        //protected class VRage_Game_GUI_TextPanel_MySpriteCollection<>Sprites<> Accessor : Network.IMemberAccessor<MySpriteCollection, MySprite[]>
        //{
        //    public VRage_Game_GUI_TextPanel_MySpriteCollection<> Sprites<>Accessor();

        //   public sealed override void Get(ref MySpriteCollection owner, out MySprite[] value);
        //    public sealed override void Set(ref MySpriteCollection owner, in MySprite[] value);
        //}
    }
}
