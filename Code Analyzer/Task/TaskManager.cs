using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

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
        public void start(ITask task, ICollection data) {
            // start workers
            foreach (var t in data)
                ThreadPool.QueueUserWorkItem(new WaitCallback(task.start), t);
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
