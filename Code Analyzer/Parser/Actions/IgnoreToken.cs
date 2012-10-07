using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Tokenizer;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Actions {
    class IgnoreToken :IAction{
        public bool exec(Token t) {
            return true;
        }
    }
}
