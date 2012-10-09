using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Kevin.CIS681.Project.CodeAnalyzer.Data.Persistence {
    [Serializable]
    class Project {
        private Dictionary<string, ArrayList> data = new Dictionary<string, ArrayList>();
    }
}
