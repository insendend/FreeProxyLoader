using System;
using System.Windows.Input;

namespace FreeProxyListLoader.ViewModels.Command
{
    abstract class CommandBase : ICommand
    {
        protected Action execute;
        protected Func<bool> canExecute;

        protected CommandBase(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentException(nameof(execute));
            this.canExecute = canExecute;
        }

        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke() ?? true;
        }

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion
    }
}
