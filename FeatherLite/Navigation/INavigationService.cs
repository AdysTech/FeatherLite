using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdysTech.FeatherLite.Navigation
{
    public interface INavigationService
    {
        bool CanGoBack
        {
            get;
        }

        Type CurrentPageType
        {
            get;
        }

        void GoBack();
        void NavigateTo(Uri pageUri);
    }
}
