using System;
using System.Threading.Tasks;

namespace SuperCore.Async.SyncContext
{
    public abstract class SuperSyncContext
    {
        public abstract void Wait(Task task);

        public abstract void Invoke(Action act);
    }
}
