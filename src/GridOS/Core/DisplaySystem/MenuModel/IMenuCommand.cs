using System;

namespace IngameScript
{
    /// <summary>
    /// Extends the basic menu item with the option to execute a command that is bound to the item.
    /// </summary>
    public interface IMenuCommand : IMenuItem
    {
        /// <summary>
        /// Notification dispatched after successful execution.
        /// </summary>
        event Action<IMenuCommand> Executed;

        /// <summary>
        /// Notification dispatched before execution commenced.
        /// </summary>
        event Action<IMenuCommand> BeforeExecute;

        /// <summary>
        /// Executes the command that is bound to the menu item.
        /// </summary>
        /// <param name="context">Identifies which display instance this operation belongs to.</param>
        void Execute(object context);
    }
}
