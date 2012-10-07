using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kevin.CIS681.Project.CodeAnalyzer.Task {
    interface ITask {
        // start one task, return false when the task is unable to be started.
        void start(object state);
    }
}
