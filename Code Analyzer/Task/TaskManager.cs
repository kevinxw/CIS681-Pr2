using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kevin.CIS681.Project.CodeAnalyzer.Task {
    class TaskManager {
        private Thread[] workers = null;
        public readonly int workerNumber; // how many worker thread should be started.  cannot be changed during program running

        
        public TaskManager(int workerNumber=10) {
            this.workerNumber = workerNumber;   // default 10 thread
        }
        // start tasks
        public void start() {
            workers = new Thread[workerNumber];
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
