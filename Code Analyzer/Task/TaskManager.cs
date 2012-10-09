using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace Kevin.CIS681.Project.CodeAnalyzer.Task {
    class TaskManager {
        private int _workerNumber=10; // how many worker thread should be started.  cannot be changed during program running

        public TaskManager(int workerNumber = 10) {
            _workerNumber = workerNumber;   // default 10 thread
            
            // set the min and max thread number here
            ThreadPool.SetMaxThreads(workerNumber, workerNumber*2);
            ThreadPool.SetMinThreads((int)(workerNumber / 2), workerNumber);
        }
        // start tasks
        public WaitHandle[] start(ITask task, ICollection data) {
            List<WaitHandle> wh = new List<WaitHandle>();
            // start workers
            foreach (var t in data) {
                TaskData td = new TaskData(t);
                wh.Add(td.resetEvent);
                ThreadPool.QueueUserWorkItem(new WaitCallback(task.start), td);
            }
            return wh.ToArray();
        }

        public static void waitAll(WaitHandle[] handles) {
            if (handles == null)
                throw new ArgumentNullException("WaitHandle should be null");
            foreach (WaitHandle wh in handles)
                wh.WaitOne();
        }

        public int threadNumber {
            get {
                return _workerNumber;
            }
        }

        // pause tasks
        public void pause() {
        }
        // abort tasks
        public void abort() {
        }
        // dispose thrad resource
        private void dispose() {
        }
    }
}
