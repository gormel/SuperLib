using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SuperCore.Async.SyncContext
{
    public class SuperWpfSyncContext : SuperSyncContext
    {
        private readonly Dispatcher mCreationDispatcher = Dispatcher.CurrentDispatcher;

        public override void Invoke(Action act)
        {
            mCreationDispatcher.BeginInvoke(DispatcherPriority.Normal, act);
        }

        public override void Wait(Task task)
        {
            var frame = new DispatcherFrame();
            var tas = task.ContinueWith(t =>
            {
                frame.Continue = false;
            });
            Dispatcher.PushFrame(frame);
        }
    }
}
