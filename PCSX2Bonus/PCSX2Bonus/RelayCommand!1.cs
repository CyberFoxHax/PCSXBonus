namespace PCSX2Bonus {
	using System;
	using System.Diagnostics;
	using System.Windows.Input;

	public sealed class RelayCommand<T> : ICommand {
		private readonly Predicate<T> _canExecute;
		private readonly Action<T> _execute;

		public event EventHandler CanExecuteChanged {
			add {
				CommandManager.RequerySuggested += value;
			}
			remove {
				CommandManager.RequerySuggested -= value;
			}
		}

		public RelayCommand(Action<T> execute)
			: this(execute, null) {
		}

		public RelayCommand(Action<T> execute, Predicate<T> canExecute) {
			if (execute == null) {
				throw new ArgumentNullException("execute");
			}
			_execute = execute;
			_canExecute = canExecute;
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter) {
			return _canExecute == null || _canExecute((T)parameter);
		}

		public void Execute(object parameter) {
			_execute((T)parameter);
		}
	}
}

