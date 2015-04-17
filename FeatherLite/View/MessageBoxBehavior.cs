using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using AdysTech.FeatherLite.Messaging;
using System.Diagnostics;

namespace AdysTech.FeatherLite.View
{
    public class MessageBoxBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached ();
            //remove all previous subscribers to avoid duplicate messages boxes popping up.
            MessageTopicProvider<DialogMessage>.Instance.Unregister (Identifier);
            MessageTopicProvider<DialogMessage>.Instance.Register (Identifier, ShowDialog);
        }


        protected override void OnDetaching()
        {
            base.OnDetaching ();
            MessageTopicProvider<DialogMessage>.Instance.Unregister (Identifier, ShowDialog);
        }

        private void ShowDialog(DialogMessage dm)
        {
            Debug.Assert (Dispatcher.CheckAccess ());
            var result = (AdysTech.FeatherLite.Messaging.MessageBoxResult) MessageBox.Show (dm.MessageBody == null ? Text : dm.MessageBody, dm.Heading == null ? Caption : dm.Heading, dm.Buttons == null ? Buttons : (System.Windows.MessageBoxButton) dm.Buttons);
            var callback = dm.Callback;
            if ( callback != null )
                callback (result);
        }

        public string Identifier { get; set; }
        public string Caption { get; set; }
        public string Text { get; set; }
        public System.Windows.MessageBoxButton Buttons { get; set; }

    }
}
