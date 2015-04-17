using AdysTech.FeatherLite.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdysTech.FeatherLite.Loggers
{
    public static class ExceptionLogger
    {

        public static async Task<string> LogExceptionAsync(Exception e)
        {
            var filename = new StringBuilder ().Append ("Exception - ").Append (DateTime.Now.ToString ("yyyy-MMM-dd-HH-mm-ss")).Append (".crashlog").ToString ();
            //await Task.Run(async ()=> await AppStorage.WriteToFileAsync (filename, GetExceptionDetails (e)));
            await AppStorage.WriteToFileAsync (filename, GetExceptionDetails (e));
            return filename;
        }

        public static string GetExceptionDetails(Exception e)
        {

            var errMsg = new StringBuilder ("Exception Tree");

            var stackTrace = new StringBuilder ();
            var inner = e;
            var innermost = e;
            do
            {
                if ( inner.Message.Length > 0 )
                    errMsg.Append (System.Environment.NewLine)
                        .Append ("----------------------------")
                        .Append (System.Environment.NewLine)
                        .Append (inner.GetType ().FullName)
                        .Append (":")
                        .Append (inner.Message);

                stackTrace.Append (System.Environment.NewLine)
                    .Append ("----------------------------")
                    .Append (System.Environment.NewLine)
                    .Append (inner.StackTrace);
                innermost = inner.InnerException ?? inner;
                inner = inner.InnerException;
            }
            while ( inner != null );
            return new StringBuilder ("-----------------Message---------------------- ")
                .Append (Environment.NewLine)
                .Append (errMsg.ToString ())
                .Append (Environment.NewLine)
                .Append ("-----------------StackTrace---------------------- ")
                .Append (Environment.NewLine)
                .Append (stackTrace.ToString())
                .ToString();
        }

        public static bool LogException(Exception excp)
        {
            var filename = "Exception - " + DateTime.Now.ToString ("yyyy-MMM-dd-HH-mm-ss") + ".crashlog";
            //await Task.Run(async ()=> await AppStorage.WriteToFileAsync (filename, GetExceptionDetails (e)));
            var excpDetails = GetExceptionDetails (excp);
            try
            {
                //Get the storage
                using ( var storage = IsolatedStorageFile.GetUserStoreForApplication () )
                {
                    //Write the file
                    using ( var stream = new StreamWriter (storage.CreateFile (filename)) )
                    {
                        stream.Write (excpDetails);
                        stream.Flush ();
                        stream.Close ();
                        return true;
                    }
                }
            }
            catch ( Exception e )
            {
                return false;
            }
        }
    }
}
