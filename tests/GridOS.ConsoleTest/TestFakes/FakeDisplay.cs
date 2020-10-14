using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    internal class FakeDisplay : IMyTextSurface
    {
        public event Action<string> TextWritten;

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
    }
}