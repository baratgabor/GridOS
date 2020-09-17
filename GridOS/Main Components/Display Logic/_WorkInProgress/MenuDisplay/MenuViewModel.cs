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
        // TODO: After this implementation is stable and feature-complete, try extracting responsibilities for scrolling and navigation into separate components.
        class MenuViewModel
        {
            public string Content => _content;
            protected string _content;

            // TODO: Implement performant update of single elements based on the logged start and end positions of elements
            protected List<ElementStartEndPositions> _contentElementLog = new List<ElementStartEndPositions>();

            protected int _selectedLine; // Selected line is viewport-relative
            protected int _lineHeight => _viewportConfig.LineHeight;

            protected int _firstMenuItemIndex;
            protected int _firstMenuItemLineIndex;
            protected StringBuilder _buffer;

            protected List<IMenuItem> _menuElements => _menuContentSource.Content as List<IMenuItem>;
            protected IMenuContentSource _menuContentSource;

            protected IAffixConfig _affixConfig;
            protected IViewportConfig _viewportConfig;

            IProcessor<IMenuItem, string> _menuItemProcessor;
            List<MenuItemPresenter> _menuItemPresenters = new List<MenuItemPresenter>();

            public MenuViewModel(
                IAffixConfig affixConfig,
                IViewportConfig viewportConfig,
                IMenuContentSource contentSource,
                IProcessor<IMenuItem, string> displayElementProcessor)
            {
                _affixConfig = affixConfig;
                _viewportConfig = viewportConfig;
                _menuContentSource = contentSource;
                _menuItemProcessor = displayElementProcessor;

                _menuContentSource.ContentChanged += OnContentChanged;
                GenerateContent();
            }

            protected void GenerateContent()
            {
                MoveToTop();
                GeneratePresenters();
                _contentElementLog.Clear();
                _buffer.Clear();

                for (int elementIndex = _firstMenuItemIndex, lineIndex = _firstMenuItemLineIndex; elementIndex < _menuItemPresenters.Count; elementIndex++)
                {
                    var currentElement = _menuItemPresenters[elementIndex];
                    var copyStartPosition = currentElement.Lines[lineIndex].Start;
                    var copyEndPosition = currentElement.PresentableContent.Length;

                    var elementStartPosition = _buffer.Length;
                    _buffer.Append(currentElement.PresentableContent, copyStartPosition, copyEndPosition);
                    var elementEndPosition = _buffer.Length;
                    _buffer.AppendLine(); // TODO: Should NewLines be integrated into MenuItem processing result instead?

                    _contentElementLog.Add(
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
                _firstMenuItemIndex = 0;
                _firstMenuItemLineIndex = 0;
            }

            protected void GeneratePresenters()
            {
                // We need to substract the line index to take into account ...
                // ... how many lines of the element doesn't count, since they are out of viewport.
                int lineCounter = 0 - _firstMenuItemLineIndex;

                for (int i = _firstMenuItemIndex; i < _menuElements.Count; i++)
                {
                    var element = _menuElements[i];
                    var elementPresenter =
                        new MenuItemPresenter(
                            element,
                            _menuItemProcessor.Process(element));

                    _menuItemPresenters.Add(elementPresenter);
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

            public void MoveUp(CommandItem sender, string parameter)
            {
                MoveUp();
            }

            public bool MoveUp()
            {
                throw new NotImplementedException();
                // if selectedline >= 0 scroll up, if possible
                // if selectedline < 0 simply decrease index and redraw
            }

            public void MoveDown(CommandItem sender, string parameter)
            {
                MoveDown();
            }

            public bool MoveDown()
            {
                throw new NotImplementedException();

            }

            public void Select(CommandItem sender, string parameter)
            {
                Select();
            }

            public bool Select()
            {
                throw new NotImplementedException();
                // TODO: transformation of selectedline to element
            }

            protected struct ElementStartEndPositions
            {
                public int Start;
                public int End;
            }
        }
    }
}
