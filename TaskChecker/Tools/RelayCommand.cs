using System;
using System.Windows.Input;
using TaskChecker.Interfaces;

namespace TaskChecker.Tools
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        private readonly Action<object> _executeWithParams;
        private readonly Predicate<object> _canExecuteWithParams;
        private readonly IDispatcherService _dispatcherService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <exception cref="ArgumentNullException">execute</exception>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : this(execute, canExecute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <exception cref="ArgumentNullException">execute</exception>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute, IDispatcherService dispatcherService)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _executeWithParams = execute;
            _canExecuteWithParams = canExecute;
            _dispatcherService = dispatcherService;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
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

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
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

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        /// <returns>
        ///   <see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute != null) return _canExecute();
            return _canExecute == null || _canExecuteWithParams(parameter);
        }

        /// <summary>
        /// Notifies the can execute changed.
        /// </summary>
        private void NotifyCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Raises the can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (_dispatcherService == null) return;
            _dispatcherService.Dispatch(new Action(NotifyCanExecuteChanged));
        }
    }
}