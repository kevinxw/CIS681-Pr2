
/*
 * An Action is what will be triggered when specific rule is found
 * Its duty is to change / mark Tokens, tell Analyzer what to do to this Token next
 * which, can also been taken as a progress of refining the data structure
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Tokenizer;
using System.Text;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Actions {
    interface IAction {
        bool exec(Token t);
    }
}
