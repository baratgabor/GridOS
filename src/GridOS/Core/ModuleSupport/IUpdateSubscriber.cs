namespace IngameScript
{
    /// <summary>
    /// Specifies a GridOS module class as a subscriber to recurring execution, with the specified update type (frequency).
    /// </summary>
    public interface IUpdateSubscriber
    {
        /// <summary>
        /// The name of the module.
        /// </summary>
        string ModuleDisplayName { get; }

        /// <summary>
        /// The frequency of the recurring execution.
        /// </summary>
        ObservableUpdateFrequency Frequency { get; }

        /// <summary>
        /// The method to be executed.
        /// </summary>
        void Update(UpdateType updateType);
    }
}
