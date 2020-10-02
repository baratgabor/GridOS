using System;

namespace IngameScript
{
    partial class Program
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

            public void Execute()
            {
                BeforeExecute?.Invoke(this);
                _command?.Invoke();
                Executed?.Invoke(this);
            }
        }
    }
}
