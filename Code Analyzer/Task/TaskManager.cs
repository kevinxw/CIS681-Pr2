using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kevin.CIS681.Project.CodeAnalyzer.Task {
    class TaskManager {
        public readonly int workerNumber; // how many worker thread should be started.  cannot be changed during program running

        public TaskManager(int workerNumber = 10) {
            this.workerNumber = workerNumber;   // default 10 thread
            
            // set the min and max thread number here
            ThreadPool.SetMaxThreads(workerNumber, workerNumber*2);
            ThreadPool.SetMinThreads((int)(workerNumber / 2), workerNumber);
        }
        // start tasks
        public void start(ITask task, object state) {
            // start workers
            for (int i = 0; i < workerNumber; i++) {
                ThreadPool.QueueUserWorkItem(new WaitCallback(task.start), state);
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
