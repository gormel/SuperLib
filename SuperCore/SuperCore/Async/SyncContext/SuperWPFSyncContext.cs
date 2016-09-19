using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SuperCore.Async.SyncContext
{
    public class SuperWPFSyncContext : SuperSyncContext
    {
        private Dispatcher mCreationDispatcher = Dispatcher.CurrentDispatcher;
        private DispatcherFrame mFrame;

        public override void Invoke(Action act)
        {
            mCreationDispatcher.BeginInvoke(DispatcherPriority.Background, act);
        }

        public override void Wait(Task task)
        {
            mFrame = new DispatcherFrame();
            task.ContinueWith(t =>
            {
                mFrame.Continue = false;
            });
            Dispatcher.PushFrame(mFrame);
        }
    }
}
