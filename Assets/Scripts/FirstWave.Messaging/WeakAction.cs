using System;

namespace FirstWave.Messaging
{
    public class WeakAction
    {
        public WeakReference Recipient { get; private set; }
        public Action ActionReference { get; private set; }

        public WeakAction(object recipient, Action action)
        {
            Recipient = new WeakReference(recipient);
            ActionReference = action;
        }
    }
}
