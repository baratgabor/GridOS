using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IngameScript
{
    internal class FakeDisplay : IMyTextSurface
    {
        public event Action<string> TextWritten;
        private List<MySprite> _spritesReceived = new List<MySprite>();

        public float FontSize { get; set; }

        public float TextPadding { get; set; }
        public bool PreserveAspectRatio { get; set; }
        public ContentType ContentType { get; set; }
        public string Script { get; set; }
        public string Font { get; set; }
        public float ChangeInterval { get; set; }
        public byte BackgroundAlpha { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string CurrentlyShownImage { get; set; }
        public Color ScriptForegroundColor { get; set; }
        public Color ScriptBackgroundColor { get; set; }

        public Vector2 TextureSize => new Vector2() { X = 400, Y = 200 };

        public Vector2 SurfaceSize => new Vector2() { X = 400, Y = 100 };

        public TextAlignment Alignment { get; set; } = TextAlignment.LEFT;
        public Color BackgroundColor { get; set; } = Color.Black;
        public Color FontColor { get; set; } = Color.White;

        public void AddImagesToSelection(List<string> ids, bool checkExistence = false)
        {
            throw new System.NotImplementedException();
        }

        public void AddImageToSelection(string id, bool checkExistence = false)
        {
            throw new System.NotImplementedException();
        }

        public void ClearImagesFromSelection()
        {
            throw new System.NotImplementedException();
        }

        public void GetFonts(List<string> fonts)
        {
            throw new System.NotImplementedException();
        }

        public void GetScripts(List<string> scripts)
        {
            throw new System.NotImplementedException();
        }

        public void GetSelectedImages(List<string> output)
        {
            throw new System.NotImplementedException();
        }

        public void GetSprites(List<string> sprites)
        {
            throw new System.NotImplementedException();
        }

        public string GetText()
        {
            throw new System.NotImplementedException();
        }

        public void ReadText(StringBuilder buffer, bool append = false)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveImageFromSelection(string id, bool removeDuplicates = false)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveImagesFromSelection(List<string> ids, bool removeDuplicates = false)
        {
            throw new System.NotImplementedException();
        }

        public bool WriteText(StringBuilder value, bool append = false)
        {
            throw new System.NotImplementedException();
        }

        public bool WriteText(string value, bool append = false)
        {
            TextWritten?.Invoke(value);
            return true;
        }

        public Vector2 MeasureStringInPixels(StringBuilder text, string font, float scale)
        {
            var fakeCharWidth = 10 * scale;
            var fakeCharHeight = 10 * scale;

            return new Vector2()
            {
                X = text.Length * fakeCharWidth,
                Y = text.Length == 0 ? 0 : fakeCharHeight * (text.ToString().Count(x => x == '\n') + 1)
            };
        }

        public MySpriteDrawFrame DrawFrame()
        {
            // TODO: This makes the assumption that 'draw frame' is supposed to be requested only once per frame. Would be more robust to wire some "frame start" notification into this fake class.
            _spritesReceived.Clear();

            return new MySpriteDrawFrame(
                spriteReceiver: (sprite) => _spritesReceived.Add(sprite),

                // Simply extract text from text type sprites.
                disposeReceiver: () => TextWritten?.Invoke(
                    string.Join(Environment.NewLine,
                    _spritesReceived
                        .Where(s => s.Type == SpriteType.TEXT)
                        .Select(s => s.Data)))); 
        }
    }
}