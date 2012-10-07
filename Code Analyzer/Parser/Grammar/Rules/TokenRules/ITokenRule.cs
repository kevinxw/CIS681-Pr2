using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Tokenizer;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.Rules.TokenRules {
    interface ITokenRule :IRule {
        void handle(Token t);
    }
}
