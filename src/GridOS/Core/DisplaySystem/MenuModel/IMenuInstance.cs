namespace IngameScript
{

    // TODO: This doesn't feel clean, and most definitely violates architectural boundaries to inject higher level (especially representation-related) components into lower level model. But we did not find any other solution to expose instance functionality inside a model state that was designed to be shared / singleton; we need to know at some point which display is calling the model.

    /// <summary>
    /// Represents and identifies a specific instance of the menu.
    /// </summary>
    public interface IMenuInstance
    {
        /// <summary>
        /// Features available on a given menu instance.
        /// </summary>
        IMenuInstanceServices MenuInstanceServices { get; }
    }
}
