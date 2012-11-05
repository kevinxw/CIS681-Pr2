/*
 * The interface of common Parser
 * Parser should be able to read data from code files and be able to save data to analysis report
 * Also, we can get raw data (code file structure), which is XML, from Parser
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser {
    interface IParser {
        void read();
        void save(string savePath);
        void save();
        XElement xElement { get; }
    }
}
