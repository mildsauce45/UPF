using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FirstWave.Messaging
{
    public class Messenger
    {
        #region Locks

        private static object creationLock = new object();
        private static object registerLock = new object();

        #endregion

        private IDictionary<string, IList<WeakAction>> recipients;

        #region Singleton Implementation

        private static Messenger instance;

        public static Messenger Default
        {
            get
            {
                if (instance == null)
                {
                    lock (creationLock)
                    {
                        instance = new Messenger();
                    }
                }

                return instance;
            }
        }

        private Messenger()
        {
            recipients = new Dictionary<string, IList<WeakAction>>();
        }

        #endregion

        public void Register(object recipient, string message, Action action)
        {
            lock (registerLock)
            {
                lock (recipients)
                {
                    if (!recipients.ContainsKey(message))
                    {
                        recipients.Add(message, new List<WeakAction>());
                    }

                    var listOfRecipients = recipients[message];

                    // Prevent the same recipient from registering for the same message twice
                    var existing = listOfRecipients.FirstOrDefault(wa => wa.Recipient.Target == recipient);
                    if (existing != null)
                        return;

                    listOfRecipients.Add(new WeakAction(recipient, action));
                }
            }
        }

        public void Unregister(object recipient)
        {
            lock (recipients)
            {
                foreach (var l in recipients.Keys)
                {
                    var list = recipients[l];

                    foreach (var wa in list.ToList())
                    {
                        if (wa.Recipient.IsAlive && wa.Recipient.Target == recipient)
                            list.Remove(wa);
                    }
                }
            }
        }

        public void SendMessage(string message)
        {
            lock (recipients)
            {
                if (recipients.ContainsKey(message))
                {
                    foreach (var wa in recipients[message])
                    {
                        if (wa.Recipient.IsAlive && wa.ActionReference != null)
                            wa.ActionReference.Invoke();
                    }
                }
            }
        }
    }
}
