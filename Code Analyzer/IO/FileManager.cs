

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.CSharp;

namespace Kevin.CIS681.Project.CodeAnalyzer.IO {
    class FileManager {
        public const string CodeFileExtKey = "file.fileExtension";

        private string[] fileExt = null;
        private ILoader grammar = null;

        public FileManager(ILoader loader) {
            grammar = loader;
            fileExt = grammar[CodeFileExtKey] as string[];
        }

        public List<string> listCodeFile(List<string> filePath, List<string> excludePath = null, bool ignoreSubDirectories = false, List<string> wildcards=null) {
            List<string> list = new List<string>();
            List<string> res = null;
            if (filePath != null)
                foreach (string path in filePath)
                    if ((res = listCodeFile(path, excludePath, ignoreSubDirectories, wildcards)) != null && res.Count > 0)
                        list.AddRange(res);
            return list;
        }

        // Process all files in the directory passed in, recurse on any directories  
        // that are found, and process the files they contain. 
        public List<string> listCodeFile(string path, List<string> excludePath = null, bool ignoreSubDirectories = false, List<string> wildcards=null) {
            // whether this path should be ignored
            if (excludePath != null) {
                string lPath = path.ToLower().Replace("/", @"\");   // convert to lower case
                foreach (string p in excludePath) {
                    string pLower = p.ToLower().Replace("/", @"\");
                    if (pLower == lPath || lPath.StartsWith(pLower + @"\")) return null;
                }
            }
            List<string> list = new List<string>();
            if (File.Exists(path)) {
                // This path is a code file
                foreach (string ext in fileExt)
                    if (path.EndsWith(ext)) {
                        if (isCodeFile(path))
                            list.Add(path);
                        break;
                    }
            }
            else if (Directory.Exists(path)) {
                List<string> _wildcard = null;
                // use wildcards
                if (wildcards != null)
                    _wildcard = wildcards;
                else {
                    _wildcard = new List<string>();
                    foreach (string ext in fileExt)
                        _wildcard.Add("*" + ext);
                };
                foreach (string ext in _wildcard) {
                    string[] fileEntries = Directory.GetFiles(path, ext, ignoreSubDirectories ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
                    foreach (string file in fileEntries)
                        if (isCodeFile(file))
                            list.Add(file);
                }
            }
            return list;
        }

        // if one file is a source code file, it is just reserved for now
        public bool isCodeFile(string path) {
            return true;
        }
    }
}
