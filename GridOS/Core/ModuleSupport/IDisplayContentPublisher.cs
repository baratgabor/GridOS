using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Specifies a GridOS module class as a publisher of display content.
        /// </summary>
        interface IDisplayContentPublisher
        {
            /// <summary>
            /// The list of display elements to register and show in the display system of GridOS.
            /// </summary>
            IEnumerable<IDisplayElement> DisplayElements { get; }
        }
    }
}
