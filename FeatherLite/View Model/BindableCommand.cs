using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AdysTech.FeatherLite.ViewModel
{
    public class BindableCommand : ICommand
    {

        private Action _execute;
        private Func<bool> _canExecute;
       

        public BindableCommand(Action execute)
            : this (execute, null)
        {
        }

        public BindableCommand(Action execute, Func<bool> canExecute)
        {
            if ( execute == null )
            {
                throw new ArgumentNullException ("execute");
            }
           
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if ( _canExecute != null )
                return _canExecute.Invoke ();
            return true;
        }

        public void Execute(object parameter)
        {
        
            if ( CanExecute (parameter) )
                _execute.Invoke ();
        }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if ( handler != null )
            {
                handler (this, EventArgs.Empty);
            }
        }

    }

    public class BindableCommand<T> : ICommand
    {

        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        public BindableCommand(Action<T> execute)
            : this (execute, null)
        {
        }

        public BindableCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if ( execute == null )
            {
                throw new ArgumentNullException ("execute");
            }
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {

            var castParameter = (T) Convert.ChangeType (parameter, typeof (T));
            if ( _canExecute != null )
                return _canExecute.Invoke (castParameter);
            return true;
        }

        public void Execute(object parameter)
        {

            if ( CanExecute (parameter) )
            {
                var castParameter = (T) Convert.ChangeType (parameter, typeof (T));
                _execute.Invoke (castParameter);
            }
        }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if ( handler != null )
            {
                handler (this, EventArgs.Empty);
            }
        }

    }
}

