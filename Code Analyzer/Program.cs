/*
 * Code Analyzer v0.1 alpha
 * for CIS681 - Proj. 1 & 2, 2012 Fall
 * Programmed on Dell 1558 (i7, 8G RAM, 300G 7200RPM), Win8
 * Target Platform : .Net Framework 3.5
 * 
 * by Kevin Wang (Xujiewen)
 * kevixw@gmail.com
 */

/*
 * Main Program
 */



#define TEST_ANALYZER     // test analyzer
//#define TEST_CMDCONSOLE   // test command line console
//#define TEST_FILEMANAGER    // test File Manager part

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Tokenizer;
using Kevin.CIS681.Project.CodeAnalyzer.UI;
using System.IO;
using Kevin.CIS681.Project.CodeAnalyzer.IO;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.CSharp;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar;
using Kevin.CIS681.Project.CodeAnalyzer.Parser;

namespace Kevin.CIS681.Project.CodeAnalyzer {
    class Program {
        private static ILoader grammarLoader = new CSharpLoader();  // load cSharp grammar

        static void Main(string[] args) {
            //Logger.enable = true;   // enable logging for debugging
#if (TEST_ANALYZER)
            testAnalyzer(new string[] {"-t",@"D:\Dropbox\Projects\CSharp",@"c:\www.aa","-p"});
#endif

#if (TEST_FILEMANAGER)
            testFileManager();
#endif

#if (TEST_CMDCONSOLE)
            testCMDConsole(new string[] {"-t",@"C:\w\s",@"c:\www.aa\2","-p"});
#endif
            while (true) ;
        }

#if (TEST_FILEMANAGER)
        private static void testFileManager() {
            Console.Out.WriteLine("File Manager Test!!");
            FileManager fm = new FileManager(new CSharpLoader());
            string[] fileEntries = fm.listCodeFile(@"D:\Dropbox\Projects\CSharp").ToArray();
            foreach (string fileName in fileEntries)
                Console.Out.WriteLine(fileName);
        }
#endif

#if (TEST_CMDCONSOLE)
        private static void testCMDConsole(string[] args) {
            Console.Out.WriteLine("Command Line Console Test!!");
            try {
                CMDConsole cmd = new CMDConsole(args);
                Console.Out.WriteLine(cmd.commands);
                Console.Out.WriteLine((cmd[CMDConsole.targetPathCMD] as List<string>)[0]);
            }
            catch (Exception e) {
                Console.Out.WriteLine(e.Message);
            }
        }
#endif

#if (TEST_ANALYZER)
        private static void testAnalyzer(string[] args) {
            Console.Out.WriteLine("Analyzer Test!!");
            CMDConsole cmd = new CMDConsole(args);
            Analyzer analyzer = new Analyzer(grammarLoader, cmd);
            analyzer.start();
        }
#endif
    }

}
