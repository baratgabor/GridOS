using System;
using System.Collections.Generic;

namespace IngameScript
{
    public interface IMenuModel
    {
        string CurrentTitle { get; }
        IReadOnlyList<IMenuItem> CurrentView { get; }
        IEnumerable<string> NavigationPath { get; }

        event Action<string> CurrentTitleChanged;
        event Action<IMenuItem> MenuItemChanged;
        event Action<IEnumerable<IMenuItem>> CurrentViewChanged;
        event Action<NavigationPayload> NavigatedTo;

        int GetIndexOf(IMenuItem item);
        void MoveBack();
        bool Select(IMenuItem item);
    }
}
