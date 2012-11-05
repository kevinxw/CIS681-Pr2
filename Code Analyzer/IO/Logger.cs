/*
 * Logger
 * Use this type to log information, present it on Console or write it to a file.
 * 
 * By default there are four types of logger created, which are Logger.info, Logger.debug, Logger.error, Logger.warn
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
        private string _saveTo = null;   // default log saving path
        public Redierction output = Redierction.Console; // log will be out put to console by default
        private StreamWriter sw = null;    // log writer
        public bool enable = true;   // enable logging
        private string _prefix = "";
        public static readonly Logger _info = new Logger("\n"),
            _debug = new Logger(false, "DEBUG: "),
            _error = new Logger("ERROR: "),
            _warn = new Logger(false, "WARNING: ");

        public Logger() { }
        public Logger(string prefix) : this() { _prefix = prefix; }
        public Logger(bool enable, string prefix) : this(prefix) { this.enable = enable; }

        public string saveTo {
            get { return _saveTo; }
            set {
                string p = Path.GetFullPath(value);
                if (Directory.Exists(p)) {
                    if (!p.EndsWith(@"\"))
                        p += "\\";
                    p += "CodeAnalyzer.info.log";
                }
                if (_saveTo == null || _saveTo.ToLower() != p.ToLower()) {
                    if (sw != null) {
                        sw.Dispose();
                        sw.Close();
                    }
                    sw = new StreamWriter(_saveTo = p, true, Encoding.UTF8);
                }
            }
        }

        public static void debug(string msg, params object[] objs) {
            // add lock here -> 11/2/2012
            lock (_debug) {
                _debug.post(msg, objs);
            }
        }
        public static void error(string msg, params object[] objs) {
            lock (_error) {
                _error.post(msg, objs);
            }
        }
        public static void warn(string msg, params object[] objs) {
            lock (_warn) {
                _warn.post(msg, objs);
            }
        }
        public static void info(string msg, params object[] objs) {
            lock (_info) {
                _info.post(msg, objs);
            }
        }
        // log a message
        public void post(string msg, params object[] objs) {
            if (!enable || msg == null)
                return;
            switch (output) {
                case Redierction.File:
                    toFile(msg, objs);
                    toConsole(msg, objs);
                    break;
                case Redierction.Console:
                    toConsole(msg, objs);
                    break;
            }
        }

        private void toConsole(string msg, params object[] objs) {
            Console.Out.WriteLine(_prefix + msg, objs);
        }

        private void toFile(string msg, params object[] objs) {
            if (_saveTo == null && output == Redierction.File)
                throw new NullReferenceException("saveTo path cannot be null!");
            if (sw == null)
                sw = new StreamWriter(_saveTo, true, Encoding.UTF8);
            sw.WriteLine(_prefix + msg, objs);
        }
    }
}
