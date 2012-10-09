/*
 * Logger
 * Use this type to log information, present it on Console or write it to a file.
 * 
 * 
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kevin.CIS681.Project.CodeAnalyzer.IO {
    class Logger {
        public enum Redierction { Console, File }
        private string _saveTo = @"C:\CodeAnalyzer.log";   // default log saving path
        public Redierction output = Redierction.Console; // log will be out put to console by default
        private StreamWriter sw = null;    // log writer
        public bool enable = true;   // enable logging
        private string _prefix = "";
        private static Logger _info = new Logger(false,"INFO: "),
            _debug = new Logger(false,"DEBUG: "),
            _error = new Logger("ERROR: "),
            _warn = new Logger(false,"WARNING: ");

        public Logger() { }
        public Logger(string prefix) : this() { _prefix = prefix; }
        public Logger(bool enable, string prefix) : this(prefix) { this.enable=enable; }

        public string saveTo {
            get { return _saveTo; }
            set {
                if (_saveTo != value)
                    sw = new StreamWriter(_saveTo = value, true, Encoding.UTF8);
            }
        }

        public static void debug(string msg, params object[] objs) {
            _debug.post(msg, objs);
        }
        public static void error(string msg, params object[] objs) {
            _error.post(msg, objs);
        }
        public static void warn(string msg, params object[] objs) {
            _warn.post(msg, objs);
        }
        public static void info(string msg, params object[] objs) {
            _info.post(msg, objs);
        }
        // log a message
        public void post(string msg, params object[] objs) {
            if (!enable || msg==null)
                return;
            switch (output) {
                case Redierction.Console:
                    toConsole(msg, objs); break;
                case Redierction.File:
                    toFile(msg, objs); break;
            }
        }

        private void toConsole(string msg, params object[] objs) {
            Console.Out.WriteLine(_prefix+msg, objs);
        }

        private void toFile(string msg, params object[] objs) {
            if (sw == null)
                sw = new StreamWriter(_saveTo, true, Encoding.UTF8);
            sw.WriteLine(_prefix+msg, objs);
        }
    }
}
