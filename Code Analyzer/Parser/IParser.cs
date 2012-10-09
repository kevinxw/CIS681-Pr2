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
