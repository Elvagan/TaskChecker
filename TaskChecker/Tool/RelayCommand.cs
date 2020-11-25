using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskChecker.Interface;

namespace TaskChecker.Tool
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        private readonly Action<object> _executeWithParams;
        private readonly Predicate<object> _canExecuteWithParams;
        private readonly IDispatcherService _dispatcherService;

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> execute)
            : this(execute, null, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : this(execute, canExecute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute, IDispatcherService dispatcherService)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _executeWithParams = execute;
            _canExecuteWithParams = canExecute;
            _dispatcherService = dispatcherService;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute == null && _canExecuteWithParams == null) return;

                CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute == null && _canExecuteWithParams == null) return;

                CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute();
            }
            else
            {
                if (_executeWithParams == null) return;

                _executeWithParams(parameter);
            }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null) return _canExecute();
            return _canExecute == null || _canExecuteWithParams(parameter);
        }

        private void NotifyCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public void RaiseCanExecuteChanged()
        {
            if (_dispatcherService == null) return;
            _dispatcherService.Dispatch(new Action(NotifyCanExecuteChanged));
        }
    }
}
