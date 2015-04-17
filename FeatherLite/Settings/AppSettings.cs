using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdysTech.FeatherLite.Settings
{
    //http://msdn.microsoft.com/en-us/library/ff769510(v=VS.92).aspx
    //no need to explicitly call .Save since WP7 will automatically save settings when app exists
    public static class AppSettings
    {
        private static Mutex threadSync = new Mutex (false, System.Guid.NewGuid ().ToString ());
        private static IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        public static bool SetValue([CallerMemberName] string Key = default(string), Object Value = null)
        {
            if ( !threadSync.WaitOne (1000) )
                throw new TimeoutException ("Unable to get hold on App Settings");

            bool valueChanged = false;
            // If the key exists
            if ( settings.Contains (Key) )
            {
                // If the value has changed
                if ( settings[Key] != Value )
                {
                    // Store the new value
                    settings[Key] = Value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add (Key, Value);
                valueChanged = true;
            }
            threadSync.ReleaseMutex ();
            return valueChanged;
        }

        // If the key exists, retrieve the value, else return default value
        public static T GetValue<T>([CallerMemberName] string Key = default(string), T DefaultValue = default(T))
        {

            if ( Key == default (string) )
                return DefaultValue;

            if ( !threadSync.WaitOne (1000) )
                throw new TimeoutException ("Unable to get hold on App Settings");

            if ( settings.Contains (Key) )
            {
                var val = (T) settings[Key];
                threadSync.ReleaseMutex ();
                return val;
            }
            else
            {
                threadSync.ReleaseMutex ();
                return DefaultValue;
            }
        }


        public static bool ContainsSetting(string Key)
        {
            if ( !threadSync.WaitOne (1000) )
                throw new TimeoutException ("Unable to get hold on App Settings");

            var present = settings.Contains (Key);
            threadSync.ReleaseMutex ();
            return present;
        }

        public static bool RemoveSetting(string Key)
        {
            if ( !threadSync.WaitOne (1000) )
                throw new TimeoutException ("Unable to get hold on App Settings");

            var present = settings.Remove (Key);
            threadSync.ReleaseMutex ();
            return present;
        }

        public static void Save()
        {
            if ( !threadSync.WaitOne (1000) )
                throw new TimeoutException ("Unable to get hold on App Settings");
            settings.Save ();
            threadSync.ReleaseMutex ();
        }


    }
}
