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
using Kevin.CIS681.Project.CodeAnalyzer.IO;
using System.Threading;
using System.Diagnostics;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser {
    class ParseTask : ITask {

        public void start(object state) {
            string filePath = state as string;
            Logger.debug("Thread {0} is parsing file {1}", Thread.CurrentThread.ManagedThreadId, filePath);
            // calculate executing time
            Stopwatch sw = new Stopwatch();
            sw.Reset(); sw.Start();
            //p = new Parser(filePath);
            IParser p = new SimpleParser(filePath);
            p.read();
            p.save();
            sw.Stop();
            Logger.debug("Thread {0}: File {1} is analyzed", Thread.CurrentThread.ManagedThreadId, filePath);
            Logger.info("Finish analyzing file {0}, {1} ms elapsed.", filePath, sw.ElapsedMilliseconds);
        }
    }
}
