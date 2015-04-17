using AdysTech.FeatherLite.Settings;
using AdysTech.FeatherLite.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdysTech.FeatherLite.Loggers
{
    public enum LogLevels
    {
        DebuggerOnly = 0,
        Debug,
        Info,
        Error
    }

    public static class ApplicationLogger
    {
        private static bool? enabled;
        private static SemaphoreSlim threadblock = new SemaphoreSlim (1);
        private static List<string> _deferredEntries = new List<string> ();
        private static bool _initialized;
        private static string _logFile = "Application.log";
        private static LogLevels _currentLevel;
        private static bool _debuggerAttached;

        public static LogLevels LogLevel
        {
            get { return AppSettings.GetValue<LogLevels> (DefaultValue: LogLevels.Debug); }
            set
            {
                if ( AppSettings.SetValue (Value: value) )
                    AppSettings.Save ();
            }
        }

        public static bool Enabled
        {
            get
            {
#if(DEBUG)
                return true;
#endif
                if ( enabled == null )
                    enabled = AppSettings.GetValue<bool> ("LoggingEnabled", false);
                return enabled.Value;
            }
        }

        public static void WriteLine(string format, LogLevels level = LogLevels.Debug, params object[] args)
        {
            string s = string.Format (format, args);
            WriteLine (s, level);
        }


        public static void WriteLine(object obj, LogLevels level = LogLevels.Debug)
        {
            if ( obj != null )
                WriteLine (obj.ToString (), level);
        }

        public static void WriteLine(Exception e, LogLevels level = LogLevels.Error)
        {
            if ( e != null )
                WriteLine (ExceptionLogger.GetExceptionDetails (e), level);
        }

        public static void WriteLine(string line, LogLevels level = LogLevels.Debug)
        {
            StringBuilder sb = new StringBuilder ();
            sb.Append (DateTime.Now.ToString ("yyyy-MMM-dd-HH-mm-ss.fffffff"));
            sb.Append ("\tTID:");
            sb.Append (System.Threading.Thread.CurrentThread.ManagedThreadId);
            sb.Append ("\t");
            sb.Append (line);
            sb.Append (Environment.NewLine);
            Task.Factory.StartNew (async () => { await LogLineAsync (sb.ToString (), level); });
        }

        private static async Task LogLineAsync(string line, LogLevels level = LogLevels.Debug)
        {
            if ( !Enabled )
                return;

            if ( !_initialized )
                await Initialize ();

            if ( _debuggerAttached )
                Debug.WriteLine (line);

            //don't write if not meeting minimum log level
            if ( _currentLevel > level ) return;

            if ( !await threadblock.WaitAsync (100) )
            {
                lock ( _deferredEntries )
                {
                    _deferredEntries.Add (line);
                }
                return;
            }
            try
            {
                List<string> list=null;
                lock ( _deferredEntries )
                {
                    if ( _deferredEntries.Count > 0 )
                    {
                        list = _deferredEntries.ToList ();
                        _deferredEntries.Clear ();
                    }
                }

                if ( list!=null)
                {
                    foreach ( var l in list)
                        await AppStorage.AppendToFileAsync (_logFile, l);                                        
                }

                await AppStorage.AppendToFileAsync (_logFile, line);
            }
            catch ( Exception e )
            {
                throw new OperationCanceledException ("Logging failed", e);
            }
            finally
            {
                threadblock.Release ();
            }
        }

        private static async Task Initialize()
        {
            if ( !_initialized )
            {
                var size = await AppStorage.GetFileSizeAsync (_logFile);
                if ( size / 1024 > 100 )
                    await AppStorage.DeleteFile (_logFile);
                _currentLevel = LogLevel;
                _debuggerAttached = Debugger.IsAttached;
                _initialized = true;

            }

        }



    }
}
