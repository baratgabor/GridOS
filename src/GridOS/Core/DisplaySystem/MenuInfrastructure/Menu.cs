using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace IngameScript
{
    /// <summary>s
    /// Draws a scrollable menu, and exposes menu navigation commands. Very deliberately not layered into traditional full content creation + offsetting, for performance reasons.
    /// This implementation efficiently fills only the menu lines displayed in the viewport, skipping the processing of all other out-of-viewport elements.
    /// This class has multiple responsibilities, arguably violates SRP, but this degree of feature density allows for an optimized solution to a specific problem.
    /// </summary>
    class Menu : IControl, IDisposable
    {
        public event Action<IControl> RedrawRequired;
        public event Action<IEnumerable<string>> NavigationPathChanged;

        private int _firstMenuItemNumber;
        private int _firstMenuItemOffset;
        private int _selectedLineIndex;

        // Alternative content generation pertaining to BuildContentFromSelectedLine(). See related TODOs.
        private readonly MenuLine[] _menuLines_alt;
        private int _selectedMenuItemIndex;
        private int _selectedMenuItemOffset;
        //

        private readonly IMenuModel _model;
        private readonly int _linesToDisplay;
        private readonly List<MenuLine> _menuLines;
        private readonly MenuLineGenerator _lineGenerator;
        private readonly StringBuilder _content = new StringBuilder();

        public Menu(IMenuModel model, MainConfig config)
        {
            this._model = model;

            _linesToDisplay = config.LineHeight;
            _lineGenerator = new MenuLineGenerator(config);
            _menuLines = new List<MenuLine>(_linesToDisplay);

            _menuLines_alt = new MenuLine[_linesToDisplay];

            model.CurrentViewChanged += OnListChanged;
            model.MenuItemChanged += OnItemChanged;
            model.NavigatedTo += OnNavigatedTo;
        }

        public void Dispose()
        {
            _model.CurrentViewChanged -= OnListChanged;
            _model.MenuItemChanged -= OnItemChanged;
            _model.NavigatedTo -= OnNavigatedTo;
        }

        public StringBuilder GetContent(bool FlushCache = false)
        {
            BuildContent();

            return _content;
        }

        public void PushUpdate()
        {
            NavigationPathChanged?.Invoke(_model.NavigationPath);
            RedrawRequired?.Invoke(this);
        }

        public void MoveUp()
        {
            // If the topmost line is selected already, try to scroll up instead.
            if (_selectedLineIndex == 0)
            {
                ScrollUp();
                RedrawRequired?.Invoke(this);
            }
            // If the selected line is larger than zero, it trivially follows that we can move up the selection.
            else
            {
                RedrawRequired?.Invoke(this);
                _selectedLineIndex--;
            }
        }

        public void MoveDown()
        {
            // If the selected line is near the end of the menu, try to scroll down instead.
            if (_selectedLineIndex == _linesToDisplay - 2)
            {
                ScrollDown();
                RedrawRequired?.Invoke(this);
            }
            // Move the selected line down, but only if we have a menu item there to select.
            else if (_menuLines.Count > _selectedLineIndex + 1)
            {
                _selectedLineIndex++;
                RedrawRequired?.Invoke(this);
            }

        }

        public void Select()
        {
            _model.Select(_menuLines[_selectedLineIndex].BackingMenuItem);
        }

        private void ScrollDown()
        {
            // Check if all the menu lines are populated. If not, we cannot scroll further.
            if (_menuLines.Count < _linesToDisplay)
            {
                return;
            }

            // If the first and second menu line belongs to the same menu item (word wrapped), we cut off a line from it at the top of the view.
            if (_menuLines[0].BackingMenuItem == _menuLines[1].BackingMenuItem)
            {
                _firstMenuItemOffset++;
            }
            // If the first and second menu line has different menu item, we remove the first item from the top of the view.
            else
            {
                _firstMenuItemOffset = 0;
                _firstMenuItemNumber++;
            }
        }

        private void ScrollUp()
        {
            // If the topmost menu item displayed has lines cut off, we put back a line.
            if (_firstMenuItemOffset > 0)
            {
                _firstMenuItemOffset--;
            }
            // If we have more menu items above our topmost menu item shown, display the last line of the previous menu item.
            else if (_firstMenuItemNumber > 0)
            {
                // TODO: Identify the number of lines of the previous menu item, and set firstMenuItemOffset to that value.
                //firstMenuItemNumber--;
                _firstMenuItemOffset = -1;
            }
        }

        private void OnListChanged(IEnumerable<IMenuItem> _)
        {
            RedrawRequired?.Invoke(this);
        }

        private void OnItemChanged(IMenuItem changedMenuItem)
        {
            // Update only if the changed menu item has at least a single line visible in the already built viewport.
            // Empty collection means content is not built yet, so we can't reason about whether an item is visible or not.
            if (_menuLines.Count > 0 && !_menuLines.Any(x => x.BackingMenuItem == changedMenuItem))
                return;

            RedrawRequired?.Invoke(this);
        }

        private void OnNavigatedTo(NavigationPayload navPayload)
        {
            // Ensure that new view is displayed from the top.
            _selectedLineIndex = 0;
            _firstMenuItemNumber = 0;
            _firstMenuItemOffset = 0;

            // Except when navigating backwards. In that case highlight the previously opened menu group label in the list.
            var previouslyOpenedGroupIndex = _model.GetIndexOf(navPayload.NavigatedFrom);
            if (previouslyOpenedGroupIndex > -1)
            {
                _selectedLineIndex = _linesToDisplay - 2; // Next to last line. Looks better if it's not at the bottom.
                _selectedMenuItemIndex = previouslyOpenedGroupIndex;
                _firstMenuItemNumber = 0;
                _firstMenuItemOffset = 0;

                BuildContentFromSelectedLine();
            }
            else
            {
                BuildContent();
            }

            NavigationPathChanged?.Invoke(_model.NavigationPath);
            RedrawRequired?.Invoke(this);
        }

        // TODO: Refactor. Implementing -1 magic int in _firstMenuItemOffset to signal above-viewport line inclusion disintegrated the structure of this method.
        private void BuildContent()
        {
            _content.Clear();
            _menuLines.Clear();

            var itemList = _model.CurrentView;
            var itemCount = itemList.Count();
            var linesGenerated = 0;

            // Menu content can change dynamically; normalize first menu item to display if it has become out of range.
            if (_firstMenuItemNumber >= itemCount && itemCount > 0)
            {
                _firstMenuItemNumber = itemCount - 1;
            }

            if (_firstMenuItemOffset == -1)
            {
                _firstMenuItemNumber--;
            }

            for (int i = _firstMenuItemNumber; i < itemCount; i++)
            {
                IEnumerable<MenuLine> menuItemLines;

                if (_firstMenuItemOffset == -1)
                {
                    menuItemLines = _lineGenerator.GetLines(itemList[i], 1);

                    _firstMenuItemOffset = menuItemLines.First().LineIndex;
                }
                else
                {
                    menuItemLines = _lineGenerator.StreamLines(itemList[i]);

                    // Special case for the first item in viewport: skip the designated number of lines to imitate continuous scrolling.
                    if (i == _firstMenuItemNumber)
                    {
                        menuItemLines = menuItemLines.Skip(_firstMenuItemOffset);
                    }
                }

                foreach (var line in menuItemLines)
                {
                    if (linesGenerated >= _linesToDisplay)
                    {
                        return;
                    }

                    _menuLines.Add(line);
                    if (linesGenerated > 0) _content.AppendLine();
                    line.AppendTo(_content, linesGenerated == _selectedLineIndex);
                    linesGenerated++;
                }
            }

            while (linesGenerated < _linesToDisplay)
            {
                if (linesGenerated > 0) _content.AppendLine();
                linesGenerated++;
            }
        }

        // TODO: Try switching to this selectedline-based content rendering instead of maintaining a hybrid solution. Also try to refactor/clean up this method for better maintainability. Oh, and menu item offsetting support still needs to be implemented.
        /// <summary>
        /// Builds content starting at the selected menu line, and working downwards and upwards separately to fill the whole viewport.
        /// </summary>
        private void BuildContentFromSelectedLine()
        {
            _content.Clear();
            _menuLines.Clear();
            Array.Clear(_menuLines_alt, 0, _menuLines_alt.Length);

            var itemList = _model.CurrentView;
            var itemCount = itemList.Count();

            // Populate lines upwards from the selected line, until reacing end of content or viewport boundary.
            var insertPosition = _selectedLineIndex;
            var lineCount = 0;
            for (int i = _selectedMenuItemIndex - 1; i >= 0; i--)
            {
                var lines = _lineGenerator.GetLines(itemList[i]);

                for (int n = lines.Count - 1; n >= 0; n--)
                {
                    insertPosition = _selectedLineIndex - 1 - lineCount;
                        
                    if (insertPosition < 0)
                        goto BreakOut1;

                    _menuLines_alt[insertPosition] = lines[n];
                    lineCount++;
                }
            }

            BreakOut1:

            // If viewport didn't get filled (i.e. there are empty lines at the top), move content to the top.
            if (insertPosition > 0)
            {
                Array.Copy(_menuLines_alt, insertPosition, _menuLines_alt, 0, _linesToDisplay - insertPosition);
                _selectedLineIndex -= insertPosition;
            }

            // Populate lines downwards from the selected line (including selected line), until reaching end of content or viewport boundary.
            lineCount = 0;
            for (int i = _selectedMenuItemIndex; i < itemCount; i++)
            {
                foreach (var line in _lineGenerator.StreamLines(itemList[i]))
                {
                    insertPosition = _selectedLineIndex + lineCount;

                    if (insertPosition >= _linesToDisplay)
                        goto BreakOut2;

                    _menuLines_alt[insertPosition] = line;
                    lineCount++;
                }
            }

            BreakOut2:

            for (int i = 0; i < _linesToDisplay; i++)
            {
                if (_menuLines_alt[i].BackingMenuItem == null)
                {
                    _content.AppendLine();
                }
                else
                {
                    if (i != 0) _content.AppendLine();
                    _menuLines_alt[i].AppendTo(_content, i == _selectedLineIndex);
                }
            }

            // Maintain compatibility with previous system
            _firstMenuItemNumber = _model.GetIndexOf(_menuLines_alt[0].BackingMenuItem);
            _menuLines.Clear();

            foreach (var line in _menuLines_alt)
            {
                if (line.BackingMenuItem != null)
                    _menuLines.Add(line);
            }

            _firstMenuItemOffset = _menuLines_alt[0].LineIndex;
            //
        }
    }
}