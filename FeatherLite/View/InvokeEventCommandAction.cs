using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace AdysTech.FeatherLite.View
{

    public sealed class InvokeEventCommandAction : TriggerAction<DependencyObject>
    {
      
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register (
            "Command", typeof (ICommand), typeof (InvokeEventCommandAction), null);

        private string commandName;

        public ICommand Command
        {
            get
            {
                return (ICommand) this.GetValue (CommandProperty);
            }
            set
            {
                this.SetValue (CommandProperty, value);
            }
        }

        public string CommandName
        {
            get
            {
                return this.commandName;
            }
            set
            {
                if ( this.CommandName != value )
                {
                    this.commandName = value;
                }
            }
        }

        //8886951379  44240000  860 834 1761
        protected override void Invoke(object parameter)
        {
            var evnt = parameter as EventArgs;

            if ( this.AssociatedObject != null )
            {
                ICommand command = this.ResolveCommand ();
                if ( ( command != null ) && command.CanExecute (evnt) )
                {
                    command.Execute (evnt);
                }
            }
        }

        private ICommand ResolveCommand()
        {
            ICommand command = null;
            if ( this.Command != null )
            {
                return this.Command;
            }
            //var frameworkElement = this.AssociatedObject as FrameworkElement;
            //if ( frameworkElement != null )
            //{
            //    object dataContext = frameworkElement.DataContext;
            //    if ( dataContext != null )
            //    {
            //        PropertyInfo commandPropertyInfo = dataContext
            //            .GetType ()
            //            .GetProperties (BindingFlags.Public | BindingFlags.Instance)
            //            .FirstOrDefault (
            //                p =>
            //                typeof (ICommand).IsAssignableFrom (p.PropertyType) &&
            //                string.Equals (p.Name, this.CommandName, StringComparison.Ordinal)
            //            );

            //        if ( commandPropertyInfo != null )
            //        {
            //            command = (ICommand) commandPropertyInfo.GetValue (dataContext, null);
            //        }
            //    }
            //}
            return command;
        }
    }



}
