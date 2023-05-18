using System;
using System.Collections.Generic;
using System.Threading;

namespace Engine
{
    public abstract class Transaction
    {
        private readonly List<Semaphore> semList = new List<Semaphore>();
        private readonly HashSet<Semaphore> semSet = new HashSet<Semaphore>();

        public virtual bool IsSingleQuery { get; } = true;

        public bool TryLock(Semaphore sem, TimeSpan timeout)
        {
            if (semSet.Add(sem))
            {
                if (sem.WaitOne(timeout))
                {
                    semList.Add(sem);
                    return true;
                }

                return false;
            }

            return true;
        }

        public void Unlock()
        {
            for (int i = semList.Count - 1; i >= 0; --i)
                semList[i].Release();
            semList.Clear();
            semSet.Clear();
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
