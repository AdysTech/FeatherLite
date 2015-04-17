using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdysTech.FeatherLite.Messaging
{
    public class MessageTopicProvider
    {
        static MessageTopicProvider _instance;
        public static MessageTopicProvider Instance
        {
            get { return _instance; }
            private set { _instance = value; }
        }

        Dictionary<string, List<Action>> topics;

        static MessageTopicProvider()
        {
            Instance = new MessageTopicProvider ();
            Instance.topics = new Dictionary<string, List<Action>> ();
        }


        #region IMessanger Members

        public bool Send(string Message)
        {
            if ( !topics.ContainsKey (Message) )
                return false;
            foreach ( var action in topics[Message].ToList () )
            {
                action.Invoke ();
            }

            return true;
        }


        public void Register(string Message, Action action)
        {
            if ( !topics.ContainsKey (Message) )
                topics.Add (Message, new List<Action> ());
            topics[Message].Add (action);
        }

        public void Unregister(string Message, Action action)
        {

            if ( topics == null )
                return;
            if ( !topics.ContainsKey (Message) )
                return;

            //TODO:Fix the unregister part
            topics[Message].RemoveAll (act => ( act.Method == action.Method ) && ( act.Target == action.Target ));
        }

        public virtual void Unregister(string Message)
        {
            if ( topics == null )
                return;
            if ( !topics.ContainsKey (Message) )
                return;
            //TODO:Fix the unregister part
            topics[Message].Clear ();
            topics.Remove (Message);
        }

        #endregion

        public void Unregister(string Message, object Subscriber)
        {

            if ( topics == null )
                return;
            if ( !topics.ContainsKey (Message) )
                return;
            //TODO:Fix the unregister part

            var subscribingactions = topics[Message].Where (act => ( act.Target == Subscriber )).ToList ();
            foreach ( var sub in subscribingactions )
                topics[Message].Remove (sub);
            if ( topics[Message].Count == 0 )
                topics.Remove (Message);
        }

        public void Unregister(object Subscriber)
        {
            if ( topics == null || topics.Count == 0 )
                return;

            lock ( topics )
            {
                //TODO:Fix the unregister part

                var subscribingmessages = topics.Where (m => m.Value.Any (act => ( act.Target == Subscriber ))).ToList ();
                foreach ( var sub in subscribingmessages )
                {
                    sub.Value.RemoveAll (act => ( act.Target == Subscriber ));
                    if ( sub.Value.Count == 0 )
                        topics.Remove (sub.Key);
                }
            }
        }
    }

    public class MessageTopicProvider<TMessageType> : MessageTopicProvider
    {
        static MessageTopicProvider<TMessageType> _instance;
        new public static MessageTopicProvider<TMessageType> Instance
        {
            get
            {
                if ( _instance == null )
                {
                    _instance = new MessageTopicProvider<TMessageType> ();
                    _instance.topics = new Dictionary<string, List<Action<TMessageType>>> ();
                }
                return _instance;
            }
        }


        Dictionary<string, List<Action<TMessageType>>> topics;



        #region IMessanger Members

        public bool Send(string Message, TMessageType MessageBody)
        {
            if ( !topics.ContainsKey (Message) )
                return false;
            foreach ( var action in topics[Message].ToList () )
            {
                action.Invoke (MessageBody);
            }

            return true;
        }


        public void Register(string Message, Action<TMessageType> action)
        {
            if ( !topics.ContainsKey (Message) )
                topics.Add (Message, new List<Action<TMessageType>> ());
            topics[Message].Add (action);

        }

        public void Unregister(string Message, Action<TMessageType> action)
        {
            if ( !topics.ContainsKey (Message) )
                return;
            //TODO:Fix the unregister part
            topics[Message].RemoveAll (p => ( p.Method == action.Method ) && ( p.Target == action.Target ));
            if ( topics[Message].Count == 0 )
                topics.Remove (Message);
        }

        #endregion

        new public void Unregister(string Message, object Subscriber)
        {
            if ( !topics.ContainsKey (Message) )
                return;

            var subscribingActions = topics[Message].Where (p => ( p.Target == Subscriber )).ToList ();
            foreach ( var sub in subscribingActions )
                topics[Message].Remove (sub);
            if ( topics[Message].Count == 0 )
                topics.Remove (Message);
        }

        public override void Unregister(string Message)
        {
            if ( topics == null )
                return;
            if ( !topics.ContainsKey (Message) )
                return;
            //TODO:Fix the unregister part
            topics[Message].Clear ();
            topics.Remove (Message);
        }

    }
}
