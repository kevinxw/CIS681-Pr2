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
        private string filePath = null;
        private Parser p;

        public void start(object state) {
            List<string>  toBeProcessedFileList = state as List<string>;
            if (toBeProcessedFileList == null || toBeProcessedFileList.Count<1)
                return;
            // get file path
            lock (toBeProcessedFileList) {
                filePath = toBeProcessedFileList[0];
                toBeProcessedFileList.RemoveAt(0);
            }
            p = new Parser(filePath);
        }
    }
}
