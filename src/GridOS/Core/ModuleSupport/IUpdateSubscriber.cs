namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Specifies a GridOS module class as a subscriber to recurring execution, with the specified update type (frequency).
        /// </summary>
        interface IUpdateSubscriber
        {
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
}
