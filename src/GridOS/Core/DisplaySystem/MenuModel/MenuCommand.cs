using System;

namespace IngameScript
{
    /// <summary>
    /// Specialized node type that holds a delegate to a method, used for executing actions.
    /// </summary>
    class MenuCommand : MenuItem, IMenuCommand
    {
        public event Action<IMenuCommand> Executed;
        public event Action<IMenuCommand> BeforeExecute;

        protected Action<object> _command;

        public MenuCommand(string label, Action<object> action) : base(label)
        {
            _command = action;
        }

        public virtual void Execute(object context)
        {
            BeforeExecute?.Invoke(this);
            _command?.Invoke(context);
            Executed?.Invoke(this);
        }
    }
}
