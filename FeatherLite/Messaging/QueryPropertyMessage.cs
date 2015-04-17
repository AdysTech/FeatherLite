using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdysTech.FeatherLite.Messaging
{
    public class QueryPropertyMessage
    {
        public QueryPropertyMessage()
        {

        }
        public QueryPropertyMessage(Action<QueryPropertyMessage> callback)
        {
            this.Callback = callback;
        }

        public Action<QueryPropertyMessage> Callback { get; set; }

        public string PropertyName { get; set; }

        object _propertyVal;
        public object PropertyValue
        {
            get { return _propertyVal; }
            set
            {
                _propertyVal = value;
                if ( Callback != null )
                    Callback.Invoke (this);
            }
        }

    }
}
