/*
 * "ParserTask" is one task that analyze one specific code file.  It will be start as a worker thread.
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.Task;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Tokenizer;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Blockader;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser {
    class ParseTask : ITask {
        private static Block src = null;    // source code file block, means it is the code file itself

        public void start() {
        }
    }
}
