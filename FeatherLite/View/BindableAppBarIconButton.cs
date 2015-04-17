using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

//few ideas taken from bindableapplicationb.codeplex.com/

namespace AdysTech.FeatherLite.View
{
    public class BindableAppBarIconButton : FrameworkElement, IApplicationBarIconButton, IApplicationBarMenuItem
    {
        //backing property
        public ApplicationBarIconButton Button { get; set; }

        public BindableAppBarIconButton()
        {
            Button = new ApplicationBarIconButton ();
            Button.Text = "Text";
            //FIx for the "Cannot clear the icon while in a list" 
            Button.IconUri = new Uri ("/Content/ApplicationBar.Add.png", UriKind.Relative);
            Button.Click += ApplBarIconButtonClick;
            Command = null;
        }

        private void ApplBarIconButtonClick(object sender, EventArgs e)
        {
            if ( Command != null && CommandParameter != null )
                Command.Execute (CommandParameter);
            else if ( Command != null )
                Command.Execute (CommandParameterValue);
            if ( Click != null )
                Click (this, e);
        }

        public int Index { get; set; }


        #region Additional Dependency properties

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached ("Command", typeof (ICommand), typeof (BindableAppBarIconButton), new PropertyMetadata (null, OnCommandChanged));

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableAppBarIconButton) d;
            ICommand oldCommand = (ICommand) e.OldValue;
            ICommand newCommand = target.Command;
            target.OnCommandChanged (oldCommand, newCommand);
        }

        private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
        {
            if ( oldCommand != null )
            {
                oldCommand.CanExecuteChanged -=
                    this.CommandCanExecuteChanged;
            }

            if ( newCommand != null )
            {
                this.IsEnabled =
                    newCommand.CanExecute (this.CommandParameter);
                newCommand.CanExecuteChanged +=
                    this.CommandCanExecuteChanged;
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            if ( this.Command != null )
            {
                this.IsEnabled =
                    this.Command.CanExecute (this.CommandParameter);
            }
        }

        public ICommand Command
        {
            get { return (ICommand) GetValue (CommandProperty); }
            set { SetValue (CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached ("CommandParameter", typeof (object), typeof (BindableAppBarIconButton), null);

        public object CommandParameter
        {
            get { return GetValue (CommandParameterProperty); }
            set { SetValue (CommandParameterProperty, value); }
        }


        public static readonly DependencyProperty CommandParameterValueProperty =
            DependencyProperty.RegisterAttached ("CommandParameterValue", typeof (object), typeof (BindableAppBarIconButton),
                new PropertyMetadata (null, OnCommandParameterChanged));

        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (BindableAppBarIconButton) d;
            object oldCommandParameter = e.OldValue;
            object newCommandParameter = target.CommandParameter;
            if ( target.Command != null )
            {
                target.IsEnabled =
                    target.Command.CanExecute (target.CommandParameter);
            }

        }

        public object CommandParameterValue
        {
            get { return GetValue (CommandParameterValueProperty); }
            set { SetValue (CommandParameterValueProperty, value); }
        }

        #endregion

        #region IApplicationBarIconButton Members

        public Uri IconUri
        {
            get { return Button.IconUri; }
            set
            {
                if ( !value.IsAbsoluteUri )
                    Button.IconUri = value;
                else
                {
                    //Fix for the designed creating an Absolute Uri from the relative path string 
                    Button.IconUri = new Uri (value.ToString ().Split ('/').Last (), UriKind.Relative);
                }
            }
        }

        #endregion

        #region IApplicationBarMenuItem Members

        public event EventHandler Click;

        public static readonly DependencyProperty IsEnabledProperty =
           DependencyProperty.RegisterAttached ("IsEnabled", typeof (bool), typeof (BindableAppBarIconButton), new PropertyMetadata (true, OnEnabledChanged));

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ( e.NewValue != e.OldValue )
            {
                ( (BindableAppBarIconButton) d ).Button.IsEnabled = (bool) e.NewValue;
            }
        }

        public bool IsEnabled
        {
            get { return (bool) GetValue (IsEnabledProperty); }
            set { SetValue (IsEnabledProperty, value); }
        }


        public static readonly DependencyProperty TextProperty =
           DependencyProperty.RegisterAttached ("Text", typeof (string), typeof (BindableAppBarIconButton), new PropertyMetadata (OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ( e.NewValue != e.OldValue )
            {
                ( (BindableAppBarIconButton) d ).Button.Text = e.NewValue.ToString ();
            }
        }
        public string Text
        {
            get { return (string) GetValue (TextProperty); }
            set { SetValue (TextProperty, value); }
        }

        public new static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.RegisterAttached ("Visibility", typeof (Visibility), typeof (BindableAppBarIconButton), new PropertyMetadata (OnVisibilityChanged));

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ( e.NewValue != e.OldValue )
            {
                var button = ( (BindableAppBarIconButton) d );
                BindableAppBar bar = button.Parent as BindableAppBar;
                bar.Invalidate ();
            }
        }

        public new Visibility Visibility
        {
            get { return (Visibility) GetValue (VisibilityProperty); }
            set { SetValue (VisibilityProperty, value); }
        }
        #endregion
    }
}
