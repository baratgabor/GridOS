namespace IngameScript
{
    /// <summary>
    /// Identifies a specific display instance to facilitate executing operations on it in shared (singleton) components.
    /// </summary>
    interface IDisplayContext
    {
        string Name { get; }
    }
}
