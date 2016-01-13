namespace PCSX2Bonus
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    public sealed class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this._execute = execute;
            this._canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (this._canExecute != null)
            {
                return this._canExecute((T) parameter);
            }
            return true;
        }

        public void Execute(object parameter)
        {
            this._execute((T) parameter);
        }
    }
}

