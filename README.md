# FeatherLite
A light weight (feather lite) toolkit to aid in MVVM development for WIndows Phone 8+ systems. Inspired by MVVMLight, but mostly independent implementation. I created this while developing Windows Phone applications, so I have not tested them in any other .Net flavors (like WPF, WinRT etc). It provides following features:

• A Bindable Object for UI and an Observable Object for Models with easy to use Property CHange notifier.
Just derive your classes from one of them, and call your setter

    bool _isInEditMode;
    public bool IsInEditMode
    {
        get { return _isInEditMode; }
        set { SetProperty (ref _isInEditMode, value); }
    }
    
`SetProperty` will take care of raising both `INotifyPropertyChanging` and `INotifyPropertyChanged` interface clients.

• An easy to use Application settings helper which lets you save a setting and retrive it in any class as easy as 

    public static bool AutoPaused
    {
        get { return AppSettings.GetValue<bool> (DefaultValue: false); }
        set
        {
            if ( AppSettings.SetValue (Value: value) )
                AppSettings.Save ();
        }
    }

• Async helper class for storing /retrieving files to and from Application Isolated storage.

• Bindable application bar/commands

• Navigation service

*Known Issues*:
The messaging component creates a hard reference to all classes registering for messages (as an action delegate is passed as an input). I was not able to find a workaround which suited rest of my design. I just called CleanUp in every class and took care of Unregistring from messages.
