using System;
using System.Windows.Input;

namespace CMFL.MVVM
{
    public class DelegateCommand : ICommand
    {
        private readonly Func<object, bool> _canExecuteFunc;
        private readonly Action<object> _executeAction;

        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            if (execute == null) return;

            _executeAction = execute;
            _canExecuteFunc = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteFunc == null || _canExecuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            _executeAction?.Invoke(parameter);
        }
    }
}