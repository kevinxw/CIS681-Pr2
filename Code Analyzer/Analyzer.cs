/*
 * This is the analyzer that dispatch code analysis tasks
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.UI;
using Kevin.CIS681.Project.CodeAnalyzer.IO;
using System.Threading;
using Kevin.CIS681.Project.CodeAnalyzer.Task;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using Kevin.CIS681.Project.CodeAnalyzer.Parser;
using System.Collections;

namespace Kevin.CIS681.Project.CodeAnalyzer {
    class Analyzer {
        private List<string> fList;
        private FileManager fm;
        private CMDConsole cmds = null;
        private TaskManager tm = null;

        public Analyzer(CMDConsole cmds) {
            fm = new FileManager();
            this.cmds = cmds;
            // get file that going to be processed
            fList =
                fm.listCodeFile(
                cmds[CMDConsole.targetPathCMD] as List<string>,
                cmds[CMDConsole.excludedPathCMD] as List<string>,
                ((bool)cmds[CMDConsole.noSubDirectoryCMD]),
                cmds[CMDConsole.wildcardSearchCMD] as List<string>
                );
            int threadNum = 10; // 10 worker threads by default
            if (cmds[CMDConsole.threadNumberCMD] != null)
                threadNum = Int32.Parse(cmds[CMDConsole.threadNumberCMD] as string);
            tm = new TaskManager(threadNum);
        }

        // start analyzing
        public void analyze() {
            if (fList.Count < 1) {
                Logger.error("You must specific at least one file to analyze!");
                return;
            }
            Logger.info("Start analyzing code files...  Please wait.\n");
            // all time elapsed
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            WaitHandle[] wh = tm.start(new ParseTask(), fList);
            TaskManager.waitAll(wh);
            sw.Stop();
            Logger.info("\nAll {0} files have been analyzed!  Takes {1} ms in total.", fList.Count, sw.ElapsedMilliseconds);
            Logger.info("\nAll reports has been saved! Please read *.ca.xml files for more information. (located in the same directory of your code files.)");
        }

        // compute file distance
        public void computeFileDistance() {
            if (fList.Count < 2) {
                Logger.error("You must specific at least two files to compute file distance!");
                return;
            }
            Logger.info("\nStart computing file distance... Please wait.");
            // all time elapsed
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            // generating file list table
            ArrayList fTable = new ArrayList();
            if ((bool)cmds[CMDConsole.allDistanceCMD]) {
                for (int i = 0; i < fList.Count; i++)
                    for (int j = 0; j < fList.Count; j++)
                        if (i != j)
                            fTable.Add(new string[] { fList[i], fList[j] });
            }
            else
                for (int i = 1; i < fList.Count; i++)
                fTable.Add(new string[] { fList[0], fList[i] });
            WaitHandle[] wh = tm.start(new FileDistanceTask(), fTable);
            TaskManager.waitAll(wh);
            sw.Stop();
            Logger.info("\nAll {0} file combination have been analyzed!  Takes {1} ms in total.", fTable.Count, sw.ElapsedMilliseconds);
        }

        // return file list
        public List<string> fileList {
            get { return fList; }
        }

        // save project
        public void save(string path = "./") {
            path = Path.GetFullPath(path);
        }
    }
}
