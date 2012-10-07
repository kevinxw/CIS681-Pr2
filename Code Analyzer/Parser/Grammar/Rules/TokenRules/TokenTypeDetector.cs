using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Tokenizer;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.Rules.TokenRules {
    class TokenTypeDetector : ITokenRule {
        public const string TokenTypeKey = "token.type";
        public const string TokenStatementKey = "token.statement";
        public const string TokenKeywordKey = "token.keyword";
        public const string TokenPreProssorKey = "token.preprossor";
        public const string TokenParameterKey = "token.parameter";

        private ILoader _grammar = null;


        private string[] _types = null, _keywords = null, _preprossor = null, _statement = null, _parameter=null;

        public
        void loadGrammar(ILoader loader) {
            _grammar = loader;
            _types = loader[TokenTypeKey] as string[];
            _keywords = loader[TokenKeywordKey] as string[];
            _statement = loader[TokenStatementKey] as string[];
            _preprossor = loader[TokenPreProssorKey] as string[];
            _parameter = loader[TokenParameterKey] as string[];
        }
        public void handle(Token t) {
            string content = t.content;
            if (_types.Contains(content))
                t["type"] = TokenTypeKey;
            else if (_statement.Contains(content))
                t["type"] = TokenStatementKey;
            else if (_preprossor.Contains(content))
                t["type"] = TokenPreProssorKey;
            else if (_keywords.Contains(content))
                t["type"] = TokenKeywordKey;
            else if (content.StartsWith(_parameter[0]) && content.EndsWith(_parameter[1]))
                t["type"] = TokenParameterKey;
        }
    }
}
