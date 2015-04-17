using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AdysTech.FeatherLite.View
{
    public class BindableAppBarMenuItem : FrameworkElement, IApplicationBarMenuItem
    {

        //backing property
        public ApplicationBarMenuItem MenuItem { get; set; }

        public BindableAppBarMenuItem()
        {
            MenuItem = new ApplicationBarMenuItem ();
            MenuItem.Text = "Text";
            MenuItem.Click += ApplicationBarMenuItemClick;
        }

        private void ApplicationBarMenuItemClick(object sender, EventArgs e)
        {
            if ( Command != null && CommandParameter != null )
                Command.Execute (CommandParameter);
            else if ( Command != null )
                Command.Execute (CommandParameterValue);
            if ( Click != null )
                Click (this, e);
        }

        #region Additional Dependency properties

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached ("Command", typeof (ICommand), typeof (BindableAppBarMenuItem), null);

        public ICommand Command
        {
            get { return (ICommand) GetValue (CommandProperty); }
            set { SetValue (CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached ("CommandParameter", typeof (object), typeof (BindableAppBarMenuItem), null);

        public object CommandParameter
        {
            get { return GetValue (CommandParameterProperty); }
            set { SetValue (CommandParameterProperty, value); }
        }


        public static readonly DependencyProperty CommandParameterValueProperty =
            DependencyProperty.RegisterAttached ("CommandParameterValue", typeof (object), typeof (BindableAppBarMenuItem), null);

        public object CommandParameterValue
        {
            get { return GetValue (CommandParameterValueProperty); }
            set { SetValue (CommandParameterValueProperty, value); }
        }

        #endregion

        #region IApplicationBarMenuItem Members

        public event EventHandler Click;

        public static readonly DependencyProperty IsEnabledProperty =
           DependencyProperty.RegisterAttached ("IsEnabled", typeof (bool), typeof (BindableAppBarMenuItem), new PropertyMetadata (true, OnEnabledChanged));

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ( e.NewValue != e.OldValue )
            {
                ( (BindableAppBarMenuItem) d ).MenuItem.IsEnabled = (bool) e.NewValue;
            }
        }

        public bool IsEnabled
        {
            get { return (bool) GetValue (IsEnabledProperty); }
            set { SetValue (IsEnabledProperty, value); }
        }


        public static readonly DependencyProperty TextProperty =
           DependencyProperty.RegisterAttached ("Text", typeof (string), typeof (BindableAppBarMenuItem), new PropertyMetadata (OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ( e.NewValue != e.OldValue )
            {
                ( (BindableAppBarMenuItem) d ).MenuItem.Text = e.NewValue.ToString ();
            }
        }
        public string Text
        {
            get { return (string) GetValue (TextProperty); }
            set { SetValue (TextProperty, value); }
        }

        public new static readonly DependencyProperty VisibilityProperty =
   DependencyProperty.RegisterAttached ("Visibility", typeof (Visibility), typeof (BindableAppBarMenuItem), new PropertyMetadata (OnVisibilityChanged));

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ( e.NewValue != e.OldValue )
            {
                var button = ( (BindableAppBarMenuItem) d );
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
