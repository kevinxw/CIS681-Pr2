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
using System.Xml.Linq;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser {
    class ParseTask : ITask {

        public void start(object state) {
            TaskData td = state as TaskData;
            string filePath = td.state as string;
            Logger.debug("Thread {0} is parsing file {1}", Thread.CurrentThread.ManagedThreadId, filePath);
            // calculate executing time
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            //p = new Parser(filePath);
            IParser p = new SimpleParser(filePath);
            p.read();
            p.save();
            sw.Stop();
            Logger.debug("Thread {0}: File {1} is analyzed", Thread.CurrentThread.ManagedThreadId, filePath);

            // get overview information about this file
            XElement xElem = p.xElement;
            Logger.info("Finish analyzing file {0}, {1} ms elapsed. {2} Namespaces, {3} Classes, {4} Methods are found.", filePath, sw.ElapsedMilliseconds,
                (from e in xElem.Descendants("elem") where e.Attribute("type").Value == SimpleParser.ELEM_NAMESPACE select e).Count(),
                (from e in xElem.Descendants("elem") where e.Attribute("type").Value == SimpleParser.ELEM_CLASS select e).Count(),
                (from e in xElem.Descendants("elem") where e.Attribute("type").Value == SimpleParser.ELEM_METHOD select e).Count());

            // finish
            td.resetEvent.Set();
        }
    }
}
