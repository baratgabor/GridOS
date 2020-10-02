namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Marker interface for classes used as GridOS modules.
        /// </summary>
        interface IModule
        {
            /// <summary>
            /// The display name of the module.
            /// </summary>
            string ModuleDisplayName { get; }
        }
    }
}
