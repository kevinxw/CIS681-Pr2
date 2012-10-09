/*
 * The data structure passed to TaskManager in multi-thread operation
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kevin.CIS681.Project.CodeAnalyzer.Task {
    class TaskData {
        private EventWaitHandle _re = new ManualResetEvent(false);    // event delegate
        private object _state;  // data

        public object state {
            get {
                return _state;
            }
        }

        public EventWaitHandle resetEvent {
            get {
                return _re;
            }
        }

        public TaskData(object data) {
            _state = data;
        }
    }
}
