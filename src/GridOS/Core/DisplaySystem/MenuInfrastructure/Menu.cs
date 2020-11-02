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
    /// Also, I included plenty of comments to aid maintenability, since the code here is less self-explanatory than normally.
    /// </summary>
    class Menu : Control, IDisposable
    {
        public event Action<IEnumerable<string>> NavigationPathChanged;

        /// <summary>
        /// Indicates if the content has been already built to reflect current model state and/or the requested navigational operations.
        /// </summary>
        private bool _isStateDirty = true;

        // Menu display context is stored by the combination of selected line, item in selected line, and item offset (in case of word-wrapped items).
        // This allows us to ensure stable selection even if the menu items are changing.
        // TODO: Track menu item instance too to ensure stability if menu items are removed.
        // TODO: Add correction logic if selected item is changed and line offset becomes invalid.
        private int _selectedLineIndex;
        private int _selectedMenuItemIndex;
        private int _selectedMenuItemOffset;

        /// <summary>
        /// Stores each line of the menu with all necessary metadata.
        /// </summary>
        private MenuLine[] _menuLines;

        /// <summary>
        /// Stores the final string representation of the menu.
        /// </summary>
        private readonly StringBuilder _menuContent = new StringBuilder();

        /// <summary>
        /// Specifies how many menu lines to produce. If menu content is shorter, it will be padded by empty lines.
        /// </summary>
        private int _lineHeight;
        private const int MinimumLineHeight = 3;

        private readonly IMenuModel _model;
        private readonly IMenuPresentationConfig _config;
        private readonly MenuLineGenerator _lineGenerator;

        public Menu(IMenuModel model, IMenuPresentationConfig config, IWordWrapper wordWrapper)
        {
            PaddingUnit = SizeUnit.Em;
            Padding = new Thickness(0.8f, 0.8f, 0, 0);

            _model = model;
            model.CurrentViewChanged += OnListChanged;
            model.MenuItemChanged += OnItemChanged;
            model.NavigatedTo += OnNavigatedTo;

            _config = config;
            config.SettingChanged += OnConfigChanged;

            _lineGenerator = new MenuLineGenerator(config, wordWrapper);

            SetUpMenuLinesCount();
        }

        public void Dispose()
        {
            _config.SettingChanged -= OnConfigChanged;
            _model.CurrentViewChanged -= OnListChanged;
            _model.MenuItemChanged -= OnItemChanged;
            _model.NavigatedTo -= OnNavigatedTo;
        }

        public override StringBuilder GetContent(bool FlushCache = false)
        {
            BuildContent();

            return _menuContent;
        }

        public void PushUpdate()
        {
            NavigationPathChanged?.Invoke(_model.NavigationPath);
            OnRedrawRequired();
        }

        /// <summary>
        /// Moves up in the menu, if possible, either by moving the line selector up, or scrolling the content up.
        /// </summary>
        public void MoveUp()
        {
            // Selection only resides in the first line if we're at the very top of the menu.
            if (_selectedLineIndex == 0)
                return;

            // Navigation requires fresh menu content to work with.
            if (_isStateDirty)
                BuildContent();

            // Second line from the top is designated for scrolling upwards.

            var lineAboveViewport = _selectedMenuItemIndex > 1 
                || _menuLines[0].LineIndex > 0 
                || (_menuLines[1].BackingMenuItem == _menuLines[2].BackingMenuItem && _selectedMenuItemIndex == 1);
            if (_selectedLineIndex == 1 && lineAboveViewport)
            {
                if (ScrollUp())
                {
                    _isStateDirty = true;
                    OnRedrawRequired();
                }
            }
            // Selected line > 1 means we'll move the selection up, either by decreasing the offset, or jumping to the last line of the preceding item.
            else
            {                
                // Moving the cursor up to the previous item/line is the combination of
                // selecting the previous line plus decrementing the selection index.
                SelectPreviousLine();
                _selectedLineIndex--;

                _isStateDirty = true;
                OnRedrawRequired();
            }
        }

        /// <summary>
        /// Moves down in the menu, if possible, either by moving the line selector down, or scrolling the content down.
        /// </summary>
        public void MoveDown()
        {
            if (_isStateDirty)
                BuildContent();

            // Next-to-last line is designated for scrolling downwards.
            if (_selectedLineIndex == _lineHeight - 2)
            {
                if (ScrollDown())
                {
                    _isStateDirty = true;
                    OnRedrawRequired();
                }
            }
            // Move the selected line down, but only if we have a menu item there to select.
            else if (_menuLines[_selectedLineIndex + 1].BackingMenuItem != null)
            {
                // Moving the cursor down to the next item/line is the combination of
                // selecting the next line plus incrementing the selection index.
                SelectNextLine();
                _selectedLineIndex++;

                _isStateDirty = true;
                OnRedrawRequired();
            }
        }

        /// <summary>
        /// Tries to execute the currently selected menu item.
        /// </summary>
        public void Select()
        {
            if (_isStateDirty)
                BuildContent();

            _model.Select(_menuLines[_selectedLineIndex].BackingMenuItem);
        }

        /// <summary>
        /// Moves the selection downwards by keeping the same line selected and scrolling the content by one line.
        /// </summary>
        private bool ScrollDown()
        {
            // Scrolling is an easier to understand concept, but technically what happens is that
            // we select the next line while keeping the selection index the same.
            return SelectNextLine();
        }

        /// <summary>
        /// Moves the selection upwards by keeping the same line selected and scrolling the content by one line.
        /// </summary>
        private bool ScrollUp()
        {
            // Scrolling is an easier to understand concept, but technically what happens is that
            // we select the previous line while keeping the selection index the same.
            return SelectPreviousLine();
        }

        private bool SelectPreviousLine()
        {
            // If the currently selected menu item is offsetted, simply decrease the offset to imitate scrolling upward by a line.
            if (_selectedMenuItemOffset > 0)
            {
                _selectedMenuItemOffset--;
                return true;
            }
            // If it's not offsetted, but we have a menu item above, set the selection to the last line of the preceding menu item.
            else if (_selectedMenuItemIndex > 0)
            {
                _selectedMenuItemIndex--;
                _selectedMenuItemOffset = _menuLines[_selectedLineIndex - 1].LineIndex;
                return true;
            }

            return false;
        }

        private bool SelectNextLine()
        {
            // If current and next line belong to the same multi-line menu item, increase the line offset to imitate line-by-line scrolling.
            if (_menuLines[_selectedLineIndex].BackingMenuItem == _menuLines[_selectedLineIndex + 1].BackingMenuItem)
            {
                _selectedMenuItemOffset++;
                return true;
            }
            // If the next line belongs to the next menu item, set the selection to the first line of the next menu item.
            else if (_selectedMenuItemIndex < _model.CurrentView.Count - 1)
            {
                _selectedMenuItemOffset = 0;
                _selectedMenuItemIndex++;
                return true;
            }

            return false;
        }

        private void OnListChanged(IEnumerable<IMenuItem> _)
        {
            _isStateDirty = true;
            OnRedrawRequired();
        }

        private void OnItemChanged(IMenuItem changedMenuItem)
        {
            // Update only if the changed menu item has at least a single line visible in the already built viewport.
            // Empty collection means content is not built yet, so we can't reason about whether an item is visible or not.
            if (_menuContent.Length > 0 && !_menuLines.Any(x => x.BackingMenuItem == changedMenuItem))
                return;

            _isStateDirty = true;
            OnRedrawRequired();
        }

        private void OnNavigatedTo(NavigationPayload navPayload)
        {
            // Ensure that new view is displayed from the top.
            _selectedLineIndex = 0;
            _selectedMenuItemIndex = 0;
            _selectedMenuItemOffset = 0;

            // Except when navigating backwards. In that case highlight the previously opened menu group label in the list.
            var previouslyOpenedGroupIndex = _model.GetIndexOf(navPayload.NavigatedFrom);
            if (previouslyOpenedGroupIndex > -1)
            {
                _selectedLineIndex = _lineHeight - 2; // Next to last line. Looks better if it's not at the bottom.
                _selectedMenuItemIndex = previouslyOpenedGroupIndex;
            }

            NavigationPathChanged?.Invoke(_model.NavigationPath);
            _isStateDirty = true;
            OnRedrawRequired();
        }

        private void OnConfigChanged(string settingName)
        {
            //if (settingName == nameof(_config.))
                SetUpMenuLinesCount();
        }

        private void SetUpMenuLinesCount()
        {
            _lineHeight = 6;
            // TODO: Implement dynamic menu sizing in sprite GUI context.

            if (_selectedLineIndex > _lineHeight - 1)
                _selectedLineIndex = _lineHeight - 2;

            _menuLines = new MenuLine[_lineHeight];
            _isStateDirty = true;
        }

        /// This appears to be fairly overcomplicated, but it's the cost of optimization.
        /// With this way of rendering, the time complexity of rendering is decoupled from the actual length of the menu.
        /// Simple sequential, top to bottom viewport filling wasn't an option, because when navigating backwards, we need to select the previously opened group in the list (preferably not at the top of the viewport), without knowing how long the predecing part of the menu is.
        /// The only thing that can throw performance off is to have an extremely long (high line number) menu item hanging into the view from the top.
        private void BuildContent()
        {
            _menuContent.Clear();

            var itemList = _model.CurrentView;
            var itemCount = itemList.Count;

            int selectedMenuItemRenderStartIndex, insertionIndex;
            if (_selectedMenuItemIndex == 0)
            {
                selectedMenuItemRenderStartIndex = 0;
                _selectedLineIndex = _selectedMenuItemOffset;
            }
            else
            {
                selectedMenuItemRenderStartIndex = _selectedLineIndex - _selectedMenuItemOffset;
            }

            // Step 1: Fill viewport upwards from above the selected menu item (if necessary).
            if (_selectedMenuItemIndex > 0)
            {
                insertionIndex = selectedMenuItemRenderStartIndex;
                if (selectedMenuItemRenderStartIndex > 0)
                {
                    for (int itemIndex = _selectedMenuItemIndex - 1; itemIndex >= 0; itemIndex--)
                    {
                        var lines = _lineGenerator.GetLines(itemList[itemIndex]);

                        for (int lineIndex = lines.Count - 1; lineIndex >= 0; lineIndex--)
                        {
                            if (insertionIndex == 0)
                            {
                                itemIndex = -1;
                                break;
                            }

                            insertionIndex--;
                            _menuLines[insertionIndex] = lines[lineIndex];
                        }
                    }
                }

                // Step 2: If Step 1 resulted in empty lines at the top of the menu, move everything up. Only when we don't know how long the menu is above our selected element.
                if (insertionIndex > 0)
                {
                    Array.Copy(_menuLines, insertionIndex, _menuLines, 0, _lineHeight - insertionIndex);
                    _selectedLineIndex -= insertionIndex;
                    selectedMenuItemRenderStartIndex -= insertionIndex;
                }
            }

            // Clear old menu lines either from the top, or from below the range where we just copied to above.
            var clearableRangeStart = Math.Max(0, selectedMenuItemRenderStartIndex);
            Array.Clear(_menuLines, clearableRangeStart, _menuLines.Length - clearableRangeStart);

            // Step 3: Fill menu lines downwards starting from the selected item. Only this executes if selection is at the top (or offsetted above the top).
            insertionIndex = selectedMenuItemRenderStartIndex;
            for (int i = _selectedMenuItemIndex; i < itemCount; i++)
            {
                foreach (var line in _lineGenerator.StreamLines(itemList[i]))
                {
                    // Menu item offsetting can result in the beginning of the item residing above-viewport. Skip writing these lines.
                    if (insertionIndex < 0)
                    {
                        insertionIndex++;
                        continue;
                    }

                    _menuLines[insertionIndex] = line;
                    insertionIndex++;

                    if (insertionIndex >= _lineHeight)
                    {
                        i = itemCount;
                        break;
                    }
                }
            }

            // Step 4: Sequentially render all above assembled lines into output, adding empty lines if needed to satisfy the required number of lines.
            for (int i = 0; i < _lineHeight; i++)
            {
                if (_menuLines[i].BackingMenuItem == null)
                {
                    if (i != 0) _menuContent.AppendLine();
                }
                else
                {
                    if (i != 0) _menuContent.AppendLine();
                    _menuLines[i].AppendTo(_menuContent, i == _selectedLineIndex);
                }
            }

            _isStateDirty = false;
        }
    }
}