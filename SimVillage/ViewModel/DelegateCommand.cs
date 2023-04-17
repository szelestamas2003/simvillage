using System.Windows.Input;
using System;

namespace SimVillage.ViewModel
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object?> execute;

        private readonly Func<object?, bool>? canExecute;
        public event EventHandler? CanExecuteChanged;

        public DelegateCommand(Action<object?> execute) : this(null, execute) { }

        public DelegateCommand(Func<object?, bool>? canExecute, Action<object?> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("Command execution is disabled.");
            }
            execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
