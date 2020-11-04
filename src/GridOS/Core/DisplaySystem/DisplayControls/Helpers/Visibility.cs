namespace IngameScript
{
    enum Visibility
    {
        /// <summary>
        /// Item is visible.
        /// </summary>
        Visible,

        /// <summary>
        /// Item is invisible, but still takes up space in the layout.
        /// </summary>
        Hidden,

        /// <summary>
        /// Item is completely excluded from rendering.
        /// </summary>
        NotRendered
    }
}
