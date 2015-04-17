using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdysTech.FeatherLite.Extensions
{
    //ref: http://stackoverflow.com/questions/22864367/fire-and-forget-approach
    public static class TaskExtensions
    {
        public static async void Forget(this Task task, params Type[] acceptableExceptions)
        {
            try
            {
                await task.ConfigureAwait (false);
            }
            catch ( Exception ex )
            {
                if ( !acceptableExceptions.Contains (ex.GetType ()) )
                    throw ex;
            }
        }
    }
}
