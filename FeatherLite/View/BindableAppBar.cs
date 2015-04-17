using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

//Original Idea of Nicolas Humann, 
//http://blog.humann.info/post/2010/08/27/How-to-have-binding-on-the-ApplicationBar.aspx
//http://phone7.codeplex.com

namespace AdysTech.FeatherLite.View
{
    [ContentProperty ("Buttons")]
    public class BindableAppBar : ItemsControl, IApplicationBar
    {
        // ApplicationBar wrappé
        private readonly ApplicationBar _applicationBar;

        public BindableAppBar()
        {
            _applicationBar = new ApplicationBar ();
            _applicationBar.StateChanged += applicationBar_StateChanged;
            this.Loaded += applicationBar_Loaded;
            Height = _applicationBar.Mode == ApplicationBarMode.Default ? _applicationBar.DefaultSize : _applicationBar.MiniSize;
            MaxHeight = _applicationBar.DefaultSize;
            MinHeight = _applicationBar.MiniSize;
        }

        private void applicationBar_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //attach to main page as its ApplicationBar
            var page =
                this.GetVisualAncestors ().Where (c => c is PhoneApplicationPage).FirstOrDefault () as PhoneApplicationPage;
            if ( page != null ) page.ApplicationBar = _applicationBar;
        }

        private void applicationBar_StateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            if ( StateChanged != null )
                StateChanged (this, e);
        }


        #region ItemsControl overrides
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //don't create buttons in desing mode to avoid unneccessory error messages
            if ( System.ComponentModel.DesignerProperties.IsInDesignTool )
            {
                return;
            }
            base.OnItemsChanged (e);
            Invalidate ();
        }

        public void Invalidate()
        {
            //remove all childs and re-add them
            _applicationBar.Buttons.Clear ();
            _applicationBar.MenuItems.Clear ();
            foreach ( BindableAppBarIconButton button in Items.Where (c => c is BindableAppBarIconButton && ( (BindableAppBarIconButton) c ).Visibility == Visibility.Visible) )
            {
                _applicationBar.Buttons.Add (button.Button);
            }
            foreach ( BindableAppBarMenuItem button in Items.Where (c => c is BindableAppBarMenuItem && ( (BindableAppBarMenuItem) c ).Visibility == Visibility.Visible) )
            {
                _applicationBar.MenuItems.Add (button.MenuItem);
            }
        }

        #endregion

        #region IApplicationBar Members


        public System.Collections.IList Buttons
        {
            get { return this.Items; }
        }

        public System.Collections.IList MenuItems
        {
            get { return this.Items; }
        }

        public double DefaultSize
        {
            get { return _applicationBar.DefaultSize; }
        }


        public bool IsMenuEnabled
        {
            get
            {
                return _applicationBar.IsMenuEnabled;
            }
            set
            {
                _applicationBar.IsMenuEnabled = value;
            }
        }

        public System.Windows.Media.Color ForegroundColor
        {
            get
            {
                return _applicationBar.ForegroundColor;
            }
            set
            {
                _applicationBar.ForegroundColor = value;
            }
        }

        public System.Windows.Media.Color BackgroundColor
        {
            get
            {
                return _applicationBar.BackgroundColor;
            }
            set
            {
                _applicationBar.BackgroundColor = value;
            }
        }

        public double MiniSize
        {
            get { return _applicationBar.MiniSize; }
        }

        public double BarOpacity
        {
            get { return _applicationBar.Opacity; }
            set { _applicationBar.Opacity = value; }
        }

        #region Dependecy properties

        public static readonly DependencyProperty IsVisibleProperty =
          DependencyProperty.RegisterAttached ("IsVisible", typeof (bool), typeof (BindableAppBar), new PropertyMetadata (true, OnVisiblityChanged));

        private static void OnVisiblityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ( e.NewValue != e.OldValue )
            {
                ( (BindableAppBar) d )._applicationBar.IsVisible = (bool) e.NewValue;
            }
        }

        public bool IsVisible
        {
            get { return (bool) GetValue (IsVisibleProperty); }
            set { SetValue (IsVisibleProperty, value); }
        }


        public static readonly DependencyProperty ModeProperty =
        DependencyProperty.RegisterAttached ("Mode", typeof (ApplicationBarMode), typeof (BindableAppBar), new PropertyMetadata (ApplicationBarMode.Default, OnModeChanged));

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ( e.NewValue != e.OldValue )
            {
                ( (BindableAppBar) d )._applicationBar.Mode = (ApplicationBarMode) e.NewValue;
                ( (BindableAppBar) d ).Height = ( (BindableAppBar) d )._applicationBar.Mode == ApplicationBarMode.Default ? ( (BindableAppBar) d )._applicationBar.DefaultSize : ( (BindableAppBar) d )._applicationBar.MiniSize;

            }
        }

        public ApplicationBarMode Mode
        {
            get { return (ApplicationBarMode) GetValue (ModeProperty); }
            set { SetValue (ModeProperty, value); }
        }
        #endregion

        public event EventHandler<ApplicationBarStateChangedEventArgs> StateChanged;

        #endregion
    }
}
