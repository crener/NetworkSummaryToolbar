using System;
using System.Windows.Input;

namespace NetworkToolbar.Utility
{
    /// <summary>
    /// Generic <see cref="ICommand"/> that has it's behaviour dynamically determined by the specific command usage 
    /// </summary>
    /// <typeparam name="T">Type to provide as parameters for the command</typeparam>
    public class DelegateCommand<T> : ICommand
    {
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            m_execute = execute;
            m_canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            // ReSharper disable once MergeCastWithTypeCheck
            if(parameter is T)
            {
                // case here to avoid issues with Value types (such as enums)
                return m_canExecute?.Invoke((T)parameter) ?? true;
            }

            return false;
        }

        public void Execute(object parameter)
        {
            // ReSharper disable once MergeCastWithTypeCheck
            if(parameter is T)
            {
                // case here to avoid issues with Value types (such as enums)
                m_execute.Invoke((T)parameter);
                return;
            }

            throw new ArgumentException($"Delegate command parameter doesn't match expected parameter filter type '{typeof(T).Name}'", nameof(parameter));
        }

        public event EventHandler CanExecuteChanged;
        private Action<T> m_execute = null;
        private Func<T, bool> m_canExecute = null;
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
            : base(execute, canExecute)
        {
            
        }
    }
}