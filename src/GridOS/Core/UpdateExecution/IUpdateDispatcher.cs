namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Component responsible for registering and deregistering subscribers to regular updates, and executing regular updates.
        /// </summary>
        interface IUpdateDispatcher
        {
            /// <summary>
            /// Adds a new subscriber to the update operation with a specific update type.
            /// </summary>
            void Add(IUpdateSubscriber updateSubscriber);

            /// <summary>
            /// Removes an existing subscriber from the update operation.
            /// </summary>
            void Remove(IUpdateSubscriber updateSubscriber);

            /// <summary>
            /// Executes the update operation by dispatching it to all subscribers of the given update type.
            /// </summary>
            void Dispatch(UpdateType UpdateType);

            /// <summary>
            /// Instructs the update scheduler to stop scheduling further updates. This means our script (the entry point of GridOS) won't be executed at a regular interval.
            /// </summary>
            void DisableUpdates();

            /// <summary>
            /// Instructs the update schedulers to resume executing the script at regular intervals, where the interval is chosen dynamically depending the needs of the registered subscribers.
            /// </summary>
            void EnableUpdates();
        }
    }
}
