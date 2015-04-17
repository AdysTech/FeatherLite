using AdysTech.FeatherLite.Messaging;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace AdysTech.FeatherLite.Navigation
{

    public class NavigationService : INavigationService
    {

        private PhoneApplicationFrame _mainFrame;
        private Uri lastPageUri;
        private Uri currentPageUri;
        private PhoneApplicationPage lastPage;
        //public event NavigatingCancelEventHandler Navigating;


        private static NavigationService _instance;
        public static NavigationService Instance
        {
            get { return NavigationService._instance; }
            private set { NavigationService._instance = value; }
        }

        static NavigationService()
        {
            Instance = new NavigationService ();
        }

        public void NavigateTo(Uri pageUri, bool oneWay)
        {
            if ( EnsureMainFrame () )
            {
                _mainFrame.Dispatcher.BeginInvoke (delegate
                {
                    _mainFrame.Navigate (pageUri);
                    _mainFrame.RemoveBackEntry ();
                });
            }
        }

        public void NavigateTo(Uri pageUri)
        {
            if ( EnsureMainFrame () )
            {
                _mainFrame.Dispatcher.BeginInvoke (delegate
                 {
                     _mainFrame.Navigate (pageUri);
                 });
            }
        }

        public void GoBack()
        {
            if ( this.CanGoBack )
            {
                _mainFrame.GoBack ();
            }
        }

        private bool EnsureMainFrame()
        {
            if ( _mainFrame != null )
            {
                return true;
            }

            _mainFrame = Application.Current.RootVisual as PhoneApplicationFrame;

            return HookupEvents ();
        }

        private bool HookupEvents()
        {
            if ( _mainFrame != null )
            {
                // Could be null if the app runs inside a design tool
                _mainFrame.Navigating += (s, e) =>
                {
                    //No need to propagate event when its already cancelled.
                    if ( e.Cancel ) return;
                    var service = s as System.Windows.Navigation.NavigationService;
                    var evnt = NavigationEvent.FromNavigatingCancelEventArgs (service, e);
                    lastPageUri = evnt.FromUri;
                    MessageTopicProvider<NavigationEvent>.Instance.Send (NavigationEvent.Navigating, evnt);
                    e.Cancel = evnt.Cancel;
                };
                _mainFrame.Navigated += (s, e) =>
                {
                    var evnt = NavigationEvent.FromNavigationEventArgs (s as System.Windows.Navigation.NavigationService, e);
                    if ( evnt.FromUri == null ) evnt.FromUri = lastPageUri;
                    currentPageUri = evnt.ToUri;
                    var page = e.Content as PhoneApplicationPage;
                    if ( page != null )
                    {
                        if ( lastPage != null && lastPage != page )
                            lastPage.BackKeyPress -= page_BackKeyPress;
                        lastPage = page;
                        page.BackKeyPress += page_BackKeyPress;
                    }
                    MessageTopicProvider<NavigationEvent>.Instance.Send (NavigationEvent.Navigated, evnt);
                };

                return true;
            }

            return false;
        }

        private void page_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var evnt = NavigationEvent.FromCancelEventArgs (sender as PhoneApplicationPage, e);
            if ( evnt.FromUri == null ) evnt.FromUri = currentPageUri;
            MessageTopicProvider<NavigationEvent>.Instance.Send (NavigationEvent.BackKeyPress, evnt);
            e.Cancel = evnt.Cancel;
        }

        public bool CanGoBack
        {
            get
            {
                if ( EnsureMainFrame () && _mainFrame.BackStack.Count () > 0 )
                    return _mainFrame.CanGoBack;
                else return false;
            }
        }

        public Type CurrentPageType
        {
            get
            {
                if ( EnsureMainFrame () )
                    return _mainFrame.GetType ();
                return default (Type);
            }
        }

        public static void Start(PhoneApplicationFrame RootFrame)
        {

            if ( !Instance.EnsureMainFrame () )
            {
                Instance._mainFrame = RootFrame;
                Instance.HookupEvents ();
            }
        }

        public void ClearBackStack()
        {
            if ( EnsureMainFrame () )
                //clear the entire page stack
                while ( _mainFrame.RemoveBackEntry () != null )
                {
                    ; // do nothing
                }
        }

        public void TerminateApplication()
        {
            Application.Current.Terminate ();
        }
    }
}
