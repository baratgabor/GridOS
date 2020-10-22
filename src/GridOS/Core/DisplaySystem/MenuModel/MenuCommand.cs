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

        protected Action _command;

        public MenuCommand(string label, Action action) : base(label)
        {
            _command = action;
        }

        public virtual void Execute()
        {
            BeforeExecute?.Invoke(this);
            _command?.Invoke();
            Executed?.Invoke(this);
        }
    }
}
