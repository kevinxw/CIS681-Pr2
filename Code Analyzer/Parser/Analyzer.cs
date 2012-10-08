/*
 * This is the analyzer that dispatch code analysis tasks
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.UI;
using Kevin.CIS681.Project.CodeAnalyzer.IO;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.CSharp;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar;
using System.Threading;
using Kevin.CIS681.Project.CodeAnalyzer.Task;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser {
    class Analyzer {
        private List<string> toBeProcessedFileList , allFileList;
        private FileManager fm;
        private ILoader grammar;
        private CMDConsole cmds;
        private TaskManager tm;

        public Analyzer(ILoader loader, CMDConsole cmds) {
            grammar =loader;
            fm = new FileManager(loader);
            this.cmds = cmds;
            // get file that going to be processed
            toBeProcessedFileList = allFileList = 
                fm.listCodeFile(
                cmds[CMDConsole.targetPathCMD] as List<string>,
                cmds[CMDConsole.excludedPathCMD] as List<string>,
                ((bool)cmds[CMDConsole.noSubDirectoryCMD]),
                cmds[CMDConsole.wildcardSearchCMD] as List<string>
                );
        }

        // start analyzing
        public void start() {
            int threadNum = 10; // 10 worker threads by default
            if (cmds[CMDConsole. threadNumberCMD] != null)
                threadNum = Int32.Parse(cmds[CMDConsole.threadNumberCMD] as string);
            tm = new TaskManager(threadNum);
            tm.start(new ParseTask(), toBeProcessedFileList, allFileList.Count);
        }

    }
}
