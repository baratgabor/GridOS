using System;
using System.Collections.Generic;

namespace IngameScript
{
    public struct MySpriteDrawFrame : IDisposable
    {
        // These are own additions to the mocked struct, to be able to hook into it in fake TextSurface implementations.
        private readonly Action<MySprite> _spriteReceiver;
        private readonly Action _disposeReceiver;

        public MySpriteDrawFrame(Action<MySprite> spriteReceiver, Action disposeReceiver)
        {
            _spriteReceiver = spriteReceiver;
            _disposeReceiver = disposeReceiver;
        }

        public void Add(MySprite sprite)
        {
            _spriteReceiver?.Invoke(sprite);
        }

        public void AddRange(IEnumerable<MySprite> sprites)
        {
            foreach (var sprite in sprites) _spriteReceiver?.Invoke(sprite);
        }

        public void AddToList(List<MySprite> list) { }
        public ClearClipToken Clip(int x, int y, int width, int height) => new ClearClipToken();
        public void Dispose()
        {
            _disposeReceiver?.Invoke();
        }

        public MySpriteCollection ToCollection() => new MySpriteCollection();

        public struct ClearClipToken : IDisposable
        {
            public ClearClipToken(MySpriteDrawFrame frame) { }

            public void Dispose() { }
        }
    }
}
