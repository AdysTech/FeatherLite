using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace AdysTech.FeatherLite.Navigation
{
    public enum NavigationEvents
    {
        Navigating,
        Navigated
    }

    public enum NavigationMode
    {
        New = 0,
        Back = 1,
        Forward = 2,
        Refresh = 3,
        Reset = 4,
    }

    public sealed class NavigationContext
    {
        // Summary:
        //     Gets a collection of query string values.
        //
        // Returns:
        //     A collection that contains the query string values.
        public IDictionary<string, string> QueryString { get; private set; }
        internal NavigationContext(Uri uri)
        {
            if ( uri == null || !uri.ToString ().Contains ('?') )
            {
                QueryString = new Dictionary<string, string> ();
            }
            else
            {
                var Uri = uri.ToString ();
                QueryString = Uri.Substring (Uri.IndexOf ('?') + 1).Split ('&').ToDictionary (s => s.Split ('=')[0], s => s.Split ('=')[1]);
            }
        }
    }

    public class NavigationEvent
    {
        public static string Navigating = "Navigating";
        public static string Navigated= "Navigated";
        public static string BackKeyPress = "BackKeyPress";

        public bool IsCancelable { get; private set; }
        public bool IsNavigationInitiator { get; private set; }
        public NavigationMode NavigationMode { get; private set; }
        public Uri FromUri { get; internal set; }
        public Uri ToUri { get; private set; }
        public bool Cancel { get; set; }
        public NavigationContext NavigationContext { get; private set; }

        internal static NavigationEvent FromNavigatingCancelEventArgs(System.Windows.Navigation.NavigationService navigationService, NavigatingCancelEventArgs e)
        {
            var evnt = new NavigationEvent ()
            {
                IsCancelable = e.IsCancelable,
                IsNavigationInitiator = e.IsNavigationInitiator,
                FromUri = UriWithoutQUeryParam (navigationService.CurrentSource),
                NavigationMode = (AdysTech.FeatherLite.Navigation.NavigationMode) e.NavigationMode,
                Cancel = e.Cancel,
                ToUri = UriWithoutQUeryParam (e.Uri),
                NavigationContext = new NavigationContext (e.Uri)

            };
            return evnt;
        }

        internal static NavigationEvent FromNavigationEventArgs(System.Windows.Navigation.NavigationService navigationService, NavigationEventArgs e)
        {
            var evnt = new NavigationEvent ()
            {
                IsCancelable = false,
                IsNavigationInitiator = e.IsNavigationInitiator,
                NavigationMode = (AdysTech.FeatherLite.Navigation.NavigationMode) e.NavigationMode,
                FromUri = navigationService.BackStack.Any () ? UriWithoutQUeryParam (navigationService.BackStack.FirstOrDefault ().Source) : null,
                ToUri = UriWithoutQUeryParam (e.Uri),
                NavigationContext = new NavigationContext (e.Uri)
            };
            return evnt;
        }

        private static Uri UriWithoutQUeryParam(Uri OriginalUri)
        {
            if ( OriginalUri == null ) return null;
            var uri = OriginalUri.ToString ();
            if ( !uri.Contains ('?') )
                return OriginalUri;
            return new Uri (uri.Substring (0, uri.IndexOf ('?')), OriginalUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }

        internal static NavigationEvent FromCancelEventArgs(Microsoft.Phone.Controls.PhoneApplicationPage phoneApplicationPage, System.ComponentModel.CancelEventArgs e)
        {
            var evnt = new NavigationEvent ()
            {
                IsCancelable = true,
                IsNavigationInitiator = true,
                NavigationMode = NavigationMode.Back,
                FromUri = null,
                ToUri = null,
                NavigationContext = null
            };
            return evnt;
        }
    }
}
