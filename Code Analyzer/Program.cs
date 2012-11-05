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

//#define TEST_SIMPLEPARSER   // test simple parser
//#define TEST_TOKENIZER       // test tokenizer
//#define TEST_ANALYZER     // test analyzer
//#define TEST_CMDCONSOLE   // test command line console
//#define TEST_FILEMANAGER    // test File Manager part

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.UI;
using System.IO;
using Kevin.CIS681.Project.CodeAnalyzer.IO;
using Kevin.CIS681.Project.CodeAnalyzer.Parser;

namespace Kevin.CIS681.Project.CodeAnalyzer {
    class Program {

        static void Main(string[] args) {

            CMDConsole cmd = new CMDConsole();
            try {
                cmd.readCommands(args);
            }
            catch (Exception e) {
                Logger.error(e.Message);
                while (true) ;
            }

            if ((bool)cmd[CMDConsole.helpCMD]) {
                Logger.info(CMDConsole.helpMessage);
                Logger.info("Press ENTER to continue other commands.");
                while ((char)Console.In.Read() != '\r') ;
            }

            string path = cmd[CMDConsole.redirectInfoCMD] as string;
            if (path != null) {
                Logger.info("Log file will be saved to {0}", path);
                Logger._info.output = Logger.Redierction.File;
                Logger._info.saveTo = path;
            }

            Analyzer analyzer = new Analyzer(cmd);

            try {
                if (!(bool)cmd[CMDConsole.disableAnalyzeCMD])
                    analyzer.analyze();

                if ((bool)cmd[CMDConsole.distanceCMD] || (bool)cmd[CMDConsole.allDistanceCMD])
                    analyzer.computeFileDistance();
            }
            catch (Exception e) {
                Console.Out.WriteLine(e.Message);
            }
            while ((char)Console.In.Read() != '\r') ;


#if (TEST_SIMPLEPARSER)
            testSimpleParser(args);
#endif

#if (TEST_TOKENIZER)
            testTokenizer(args);
#endif

#if (TEST_ANALYZER)
            testAnalyzer(new string[] { "-t", @"X:\Code Analyzer", @"c:\www.aa", "-e", @"D:\Dropbox\Projects\CSharp\Parser" });
#endif

#if (TEST_FILEMANAGER)
            testFileManager();
#endif

#if (TEST_CMDCONSOLE)
            testCMDConsole(new string[] {"-t",@"C:\w\s",@"c:\www.aa\2","-p"});
#endif

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

#if (TEST_TOKENIZER)
        private static void testTokenizer(string[] args) {
            string testCodeFile = @"D:\Dropbox\Projects\CSharp\Code-Analyzer\Code Analyzer\Parser\Analyzer.cs";
            StreamReader sr = new StreamReader(testCodeFile);
            Tokenizer ter = new Tokenizer(sr);
            ter.read();
            Console.Out.WriteLine(ter.token);
        }
#endif

#if (TEST_SIMPLEPARSER)
        private static void testSimpleParser(string[] args) {
            string testCodeFile = @"D:\Dropbox\Projects\CSharp\Code-Analyzer\Code Analyzer\Parser\SimpleParser.cs";
            IParser sp = new SimpleParser(testCodeFile);
            sp.read();
            sp.save(@"X:\test.xml");
        }
#endif

    }

}
