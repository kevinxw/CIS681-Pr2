/*
 * Command Line Console
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace Kevin.CIS681.Project.CodeAnalyzer.UI {
    class CMDConsole {
        private const string CMDPrefix = "-";
        private const string windowsPathRegExStr = @"^(([a-zA-Z]:|\\)(\\(?! )[^\\/:*?""<>|]+)*|((\.{1,2}/)*(?! )[^\\/:*?""<>|]+)*)$";

        private static readonly Regex pathRegEx = new Regex(windowsPathRegExStr, RegexOptions.Compiled);

        private Dictionary<string, List<string>> _args = new Dictionary<string, List<string>>();
        // cmd dictionary is formated like this:
        /* key: "command"
         * value[0]: maxmium number of accpetable arguments,
         * value[1]: argument regEx,
         * value[2]: cmd description,
         * value[3]: is option required,
         */
        private static Dictionary<string, ArrayList> _cmds = new Dictionary<string, ArrayList>();

        public const string targetPathCMD = "t";
        public const string excludedPathCMD = "e";
        public const string projectPathCMD = "p";
        public const string helpCMD = "h";
        public const string threadNumberCMD = "th";
        public const string noSubDirectoryCMD = "ns";
        public const string wildcardSearchCMD = "w";
        public const string distanceCMD = "d";
        public const string allDistanceCMD = "ad";
        public const string disableAnalyzeCMD = "na";
        public const string redirectInfoCMD = "ri";

        private static StringBuilder _helpMsg = new StringBuilder("====== Help information ======\n\n");

        static CMDConsole() {
            // target directory
            _cmds.Add(targetPathCMD, new ArrayList() { 9999, pathRegEx, "The target directory you are going to analyze.", true });
            // excluded target directory
            _cmds.Add(excludedPathCMD, new ArrayList() { 9999, pathRegEx, "Excludes these directories or files.  Code Analyzer will ignore them when analyzing.", false });
            // do not scan sub-directories
            _cmds.Add(noSubDirectoryCMD, new ArrayList() { 0, null, "Enable this and Code Analyzer will not scan the sub-directories of the target directory.", false });
            // wildcard pattern
            _cmds.Add(wildcardSearchCMD, new ArrayList() { 9999, @"^[^\\/:""<>|]+$", "Use wildcards to match files.", false });
            // project directory
            _cmds.Add(projectPathCMD, new ArrayList() { 1, pathRegEx, "Specific where to save the analysis report.  No result will be saved if this is not specificed.", false });
            // help message
            _cmds.Add(helpCMD, new ArrayList() { 0, null, "show Help information, about how to use this software", false });
            // work thread numbers
            _cmds.Add(threadNumberCMD, new ArrayList() { 1, @"^\d+$", "set the number of threads that to be used in analyzing.  Ten threads by default if not specificed.", false });
            // if we should calculate distance
            _cmds.Add(distanceCMD, new ArrayList() { 0, null, "Enable distance compare.  When this option is specificed, Code Analyzer will calculate the distance between the first code files and all the rest found in target directory.", false });
            // disable code analyzing
            _cmds.Add(disableAnalyzeCMD, new ArrayList() { 0, null, "Disable code analyzing. When you have former reports remain not deleted and you just want to calculate file distance, you may not need to analyze all the codes again!", false });
            // get distance of all possible file combination
            _cmds.Add(allDistanceCMD, new ArrayList() { 0, null, "Be careful to enable this option! It will calculate distance of all possible file combination! That's O(n^2)!", false });
            // redirect Logger.info to file
            _cmds.Add(redirectInfoCMD, new ArrayList() { 1, pathRegEx, "Specific a path where analysis log will be saved.", false });

            // generate help message
            foreach (string k in _cmds.Keys)
                _helpMsg.AppendFormat("-{0}\t{1}. [{2}]\n", k, _cmds[k][2], (bool)_cmds[k][3] ? "Required" : "Optional");
        }

        public static string helpMessage {
            get {
                return _helpMsg.ToString();
            }
        }

        public CMDConsole() {
        }

        public CMDConsole(string[] args)
            : this() {
            readCommands(args);
        }

        public string[] commands {
            get {
                return _args.Keys.ToArray();
            }
        }

        public object this[string index] {
            get {
                int maxArgNum = (int)_cmds[index][0];
                if (maxArgNum < 1)
                    return _args.ContainsKey(index);
                else if (_args.ContainsKey(index)) {
                    if (maxArgNum == 1)
                        return _args[index][0];
                    else
                        return _args[index];
                }
                else
                    return null;
            }
        }

        public void readCommands(string[] args) {
            string lastCMD = null;
            //Regex reg = null;
            // read arguments
            for (int i = 0; i < args.Length; i++) {
                string str = null;
                if (args[i].StartsWith(CMDPrefix) && _cmds.ContainsKey(str = args[i].Substring(1))) {
                    _args[lastCMD = str] = new List<string>();
                }
                else if (lastCMD != null)
                    //if ((reg=_cmds[lastCMD][1] as Regex) != null && reg.IsMatch(args[i])) {
                        //if (_args[lastCMD].Count >= ((int)_cmds[lastCMD][0]))
                        //    throw new ArgumentException("The argument \"" + args[i] + "\" is not valid! (exceeds limitation)");
                        _args[lastCMD].Add(args[i]);
                    //}
                   // else
                    //    throw new ArgumentException("The argument \"" + args[i] + "\" is not valid!");
            }
            // check if every required argument is read
        }

    }
}
