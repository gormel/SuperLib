using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCore.Async.SyncContext
{
    public class SuperDefaultSyncContext : SuperSyncContext
    {
        public override void Invoke(Action act)
        {
            act();
        }

        public override void Wait(Task task)
        {
            task.Wait();
        }
    }
}
