using System.Collections.Generic;

namespace IngameScript
{
    /// <summary>
    /// Specifies a GridOS module class as a publisher of display content.
    /// </summary>
    interface IMenuContentPublisher
    {
        /// <summary>
        /// The list of menu items to register and show in the display system of GridOS.
        /// </summary>
        IEnumerable<IMenuItem> MenuItems { get; }
    }
}
