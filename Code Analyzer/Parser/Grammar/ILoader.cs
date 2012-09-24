using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kevin.CIS681.Project1.CodeAnalyzer.Parser.Grammar {
    interface ILoader {
        // the extension of code file
        public string[] fileExtension { get; }
        // is source code file a text file.
        public bool isTextFile { get; }
    }
}
