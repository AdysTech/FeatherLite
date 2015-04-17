using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdysTech.FeatherLite.Messaging
{
    public enum MessageBoxResult
    {
        // Summary:
        //     The message box returns no result.
        None = 0,
        //
        // Summary:
        //     The result value of the message box is OK.
        OK = 1,
        //
        // Summary:
        //     The result value of the message box is Cancel.
        Cancel = 2,
        //
        // Summary:
        //     The result value of the message box is Yes.
        Yes = 6,
        //
        // Summary:
        //     The result value of the message box is No.
        No = 7,
    }

    public enum MessageBoxButton
    {
        // Summary:
        //     The message box displays an OK button.
        OK = 0,
        //
        // Summary:
        //     The message box displays OK and Cancel buttons.
        OKCancel = 1,
        //
        // Summary:
        //     The message box displays Yes, No, and Cancel buttons.
        YesNoCancel = 3,
        //
        // Summary:
        //     The message box displays Yes and No buttons.
        YesNo = 4,
    }

    public class DialogMessage
    {
        public DialogMessage(Action<MessageBoxResult> callback)
        {
            this.Callback = callback;
        }

        public DialogMessage(string MessageBody, string MessageHeading, MessageBoxButton buttons, Action<MessageBoxResult> callback)
        {
            this.Callback = callback;
            this.MessageBody = MessageBody;
            this.Buttons = buttons;
            this.Heading = MessageHeading;
        }

        public Action<MessageBoxResult> Callback { get; private set; }

        public string Heading { get; private set; }

        public MessageBoxButton? Buttons { get; private set; }

        public string MessageBody { get; private set; }
    }
}
