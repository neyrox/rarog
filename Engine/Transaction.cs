using System;
using System.Collections.Generic;
using System.Threading;

namespace Engine
{
    public abstract class Transaction
    {
        private readonly List<object> syncObjectsList = new List<object>();
        private readonly HashSet<object> syncObjectsSet = new HashSet<object>();

        public virtual bool IsSingleQuery { get; } = true;

        public bool TryLock(object syncObject, TimeSpan timeout)
        {
            if (syncObjectsSet.Add(syncObject))
            {
                if (Monitor.TryEnter(syncObject, timeout))
                {
                    syncObjectsList.Add(syncObject);
                    return true;
                }

                return false;
            }

            return true;
        }

        public void Unlock()
        {
            for (int i = syncObjectsList.Count - 1; i >= 0; --i)
                Monitor.Exit(syncObjectsList[i]);
            syncObjectsList.Clear();
            syncObjectsSet.Clear();
        }

        public abstract void QueryEnd();
    }

    public class SingleQueryTransaction : Transaction
    {
        public override bool IsSingleQuery { get; } = true;

        public override void QueryEnd()
        {
            Unlock();
        }
    }

    public class ComplexTransaction : Transaction
    {
        public override bool IsSingleQuery { get; } = false;

        public override void QueryEnd()
        {
        }
    }
}
