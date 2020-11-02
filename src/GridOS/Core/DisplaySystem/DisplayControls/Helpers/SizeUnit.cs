namespace IngameScript
{
    public enum SizeUnit
    {
        /// <summary>
        /// Relative to the font size of the control, where 1 is 100% of the font size.
        /// </summary>
        Em,

        /// <summary>
        /// Literal pixels, where 1 is 1 pixel on the screen.
        /// </summary>
        Px,

        /// <summary>
        /// Virtual, density-independent pixels, where values are scaled to the density of the display.
        /// </summary>
        Dip,

        /// <summary>
        /// Relative to the window size, where e.g. 50 is half of the size.
        /// </summary>
        Percent
    }
}
