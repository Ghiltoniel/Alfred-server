using System.Collections.Generic;
using System.Threading;
using Alfred.Model.Core;

namespace Alfred.Server
{
	public class TaskPool
	{
        private Queue<AlfredTask> _tasks = new Queue<AlfredTask>();
        private readonly object _locker = new object();
	    private readonly EventWaitHandle _wh = new AutoResetEvent(false);

		public TaskPool()
		{
		    _wh = new AutoResetEvent(false);
            _tasks = new Queue<AlfredTask>();
			var worker = new Thread(Work) {IsBackground = true};
		    worker.Start();
		}

        public void EnqueueTask(AlfredTask task)
        {
            if (!_tasks.Contains(task))
                lock (_locker)
                    _tasks.Enqueue(task);
            _wh.Set();
        }

		private void Work()
		{
			while (true)
			{
				AlfredTask task = null;
				lock (_locker)
					if (_tasks.Count > 0)
					{
						task = _tasks.Dequeue();
						if (task == null)
							return;
					}
				if (task != null)
				{
					if (task.Command != null)
					{
                        Launcher.ExecuteServer(task);
                    }
				}
				else
					_wh.WaitOne();         // No more tasks - wait for a signal
			}
		}
	}
}
