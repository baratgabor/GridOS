using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program
    {

        class MenuProcessorPrototye
        {
            protected IAffixConfig _affixConfig;
            protected IViewportConfig _viewportConfig;
            protected List<IMenuItem> _menuElements => _menuContentSource.Content as List<IMenuItem>;
            protected IMenuContentSource _menuContentSource;
            IProcessor<IMenuItem, string> _displayElementProcessor;

            protected string[] _lines;

            public MenuProcessorPrototye(
                IAffixConfig affixConfig,
                IViewportConfig viewportConfig,
                IMenuContentSource contentSource,
                IProcessor<IMenuItem, string> displayElementProcessor)
            {
                _affixConfig = affixConfig;
                _viewportConfig = viewportConfig;
                _menuContentSource = contentSource;
                _displayElementProcessor = displayElementProcessor;

                _menuContentSource.ContentChanged += OnContentChanged;
            }

            protected void OnContentChanged(IEnumerable<IMenuItem> obj)
            {
                throw new NotImplementedException();
            }


        }

        // TODO: After this implementation is stable and feature-complete, try extracting responsibilities for scrolling and navigation into separate components.
        class MenuController
        {
            public string Content => _content;
            public int LineHeight => _viewportConfig.LineHeight;

            protected string _content;

            // TODO: Implement performant update of single elements based on the logged start and end positions of elements
            protected List<ElementStartEndPositions> _contentCompositionLog = new List<ElementStartEndPositions>();

            protected int _selectedLine; // Selected line is viewport-relative
            protected int _lineHeight => _viewportConfig.LineHeight;

            protected int _firstDisplayElementIndex;
            protected int _firstDisplayElementLineIndex;
            protected StringBuilder _buffer;

            protected List<IMenuItem> _menuElements => _menuContentSource.Content as List<IMenuItem>;
            protected IMenuContentSource _menuContentSource;

            protected IAffixConfig _affixConfig;
            protected IViewportConfig _viewportConfig;

            IProcessor<IMenuItem, string> _displayElementProcessor;
            // TODO: Rename DisplayElementPresenter, it's a crap name
            List<DisplayElementPresenter> _displayElementPresenters = new List<DisplayElementPresenter>();

            public MenuController(
                IAffixConfig affixConfig,
                IViewportConfig viewportConfig,
                IMenuContentSource contentSource,
                IProcessor<IMenuItem, string> displayElementProcessor)
            {
                _affixConfig = affixConfig;
                _viewportConfig = viewportConfig;
                _menuContentSource = contentSource;
                _displayElementProcessor = displayElementProcessor;

                _menuContentSource.ContentChanged += OnContentChanged;
                GenerateContent();
            }

            protected void GenerateContent()
            {
                GeneratePresenters();
                _contentCompositionLog.Clear();
                _buffer.Clear();

                for (int elementIndex = _firstDisplayElementIndex,
                    lineIndex = _firstDisplayElementLineIndex; 
                    elementIndex < _displayElementPresenters.Count; elementIndex++)
                {
                    var currentElement = _displayElementPresenters[elementIndex];
                    var copyStartPosition = currentElement.Lines[lineIndex].Start;
                    var copyEndPosition = currentElement.PresentableContent.Length;

                    var elementStartPosition = _buffer.Length;
                    _buffer.Append(currentElement.PresentableContent, copyStartPosition, copyEndPosition);
                    var elementEndPosition = _buffer.Length;
                    _buffer.AppendLine(); // TODO: Should NewLines be integrated into DisplayElement processing result instead?

                    _contentCompositionLog.Add(
                        new ElementStartEndPositions() {
                            Start = elementStartPosition,
                            End = elementEndPosition});

                    lineIndex = 0; // Include full content of next elements after first element is already processed
                }
                _content = _buffer.ToString();
            }

            protected void MoveToTop()
            {
                _selectedLine = 0;
                _firstDisplayElementIndex = 0;
                _firstDisplayElementLineIndex = 0;

                float f;
                float.TryParse("", System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out f);
            }

            protected void GeneratePresenters()
            {
                // We need to substract the line index to take into account ...
                // ... how many lines of the element doesn't count, since they are out of viewport.
                int lineCounter = 0 - _firstDisplayElementLineIndex;

                for (int i = _firstDisplayElementIndex; i < _menuElements.Count; i++)
                {
                    var element = _menuElements[i];
                    var elementPresenter =
                        new DisplayElementPresenter(
                            element,
                            _displayElementProcessor.Process(element));

                    _displayElementPresenters.Add(elementPresenter);
                    lineCounter += elementPresenter.Lines.Count;

                    // Stop processing next element we already have enough elements to fill the viewport
                    if (lineCounter >= _viewportConfig.LineHeight)
                        break;
                }
            }

            private void OnContentChanged(IEnumerable<IMenuItem> obj)
            {
                GenerateContent();
            }


            // TODO: Implement fast update of content by prepending the new element when moving up, instead of re-generating
            public bool MoveUp()
            {
                if (_firstDisplayElementLineIndex > 0)
                {
                    _firstDisplayElementLineIndex--;
                    GenerateContent();
                    return true;
                }

                if (_firstDisplayElementIndex > 0)
                {
                    // Move top back by one element
                    _firstDisplayElementIndex--;

                    // Set element's last line as the top
                    _firstDisplayElementLineIndex = _displayElementPresenters[_firstDisplayElementIndex].Lines.Count - 1;

                    GenerateContent();
                    return true;
                }

                return false;
            }

            // TODO: Implement fast update of content by appending the new element when moving up, instead of re-generating
            public bool MoveDown()
            {
                // Is there any line left from current element?
                if (_firstDisplayElementLineIndex < _displayElementPresenters[_firstDisplayElementLineIndex].Lines.Count-1)
                {
                    _firstDisplayElementLineIndex++;
                    GenerateContent();
                    return true;
                }

                // Is there another element after the current element?
                if (_firstDisplayElementIndex < _displayElementPresenters.Count-1)
                {
                    _firstDisplayElementIndex++;
                    _firstDisplayElementLineIndex = 0;
                    GenerateContent();
                    return true;
                }

                return false;
            }

            public void ExecuteAt(int lineNumber)
            {
                var menuItem = GetElementAt(lineNumber);
                if (menuItem is IMenuCommand)
                    ((IMenuCommand)menuItem).Execute();
            }

            protected IMenuItem GetElementAt(int lineNumber)
            {
                if (lineNumber < 0 || lineNumber >= _lineHeight)
                    throw new ArgumentException("Provided line number is invalid.");

                var lines = 0;
                IMenuItem element = null;
                foreach (var p in _displayElementPresenters)
                {
                    if (lines + p.Lines.Count < lineNumber)
                        lines += p.Lines.Count;
                    else
                    {
                        element = p.OriginalElement;
                        break;
                    }
                }

                if (element == null)
                    throw new InvalidOperationException("Element retrieval failed. No element at given line number.");

                return element;
            }

            protected struct ElementStartEndPositions
            {
                public int Start;
                public int End;
            }
        }
    }
}
