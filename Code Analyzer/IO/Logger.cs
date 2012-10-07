using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kevin.CIS681.Project.CodeAnalyzer.IO {
    class Logger {
        public enum Redierction { Console, File }
        private static string _saveTo = @"C:\CodeAnalyzer.log";   // default log saving path
        public static Redierction output = Redierction.Console; // log will be out put to console by default
        private static StreamWriter sw = new StreamWriter(_saveTo, true, Encoding.UTF8);    // log writer
        public static bool enable = false;   // enable logging

        public static string saveTo {
            get { return _saveTo; }
            set {
                if (_saveTo != value)
                    sw = new StreamWriter(_saveTo = value, true, Encoding.UTF8);
            }
        }

        // log a message
        public static void log(string msg) {
            if (!enable)
                return;
            switch (output) {
                case Redierction.Console:
                    toConsole(msg); break;
                case Redierction.File:
                    toFile(msg); break;
            }
        }

        private static void toConsole(string msg) {
            Console.Out.WriteLine("Log: " + msg);
        }

        private static void toFile(string msg) {
            sw.WriteLine(msg);
        }
    }
}
