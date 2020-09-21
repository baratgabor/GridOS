using System;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Extends the basic display element with the option to execute a command that is bound to the element.
        /// </summary>
        interface IDisplayCommand : IDisplayElement
        {
            /// <summary>
            /// Notification dispatched after successful execution.
            /// </summary>
            event Action<IDisplayCommand> Executed;

            /// <summary>
            /// Notification dispatched before execution commenced.
            /// </summary>
            event Action<IDisplayCommand> BeforeExecute;

            /// <summary>
            /// Executes the command that is bound to the element.
            /// </summary>
            void Execute();
        }
    }
}
