/*
 * This is a universal detector that determine whether a token is completed
 * (Well, not that universal, it is somehow more for C#)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.Rules.TokenRules {
    class TokenIntegrityDetector : ITokenizerRule {
        public const string PairedTokenDelimitersKey = "token.pairedTokenDelimiters";
        public const string EscapeCharKey = "token.escapeChar";
        public const string VerbatimCharKey = "token.verbatimChar";
        public const string LegalVariableRegExKey = "token.legalVariableRegEx";
        public const string LegalNumericRegExKey = "token.legalNumericRegEx";
        private const char emptyChar = '\0';

        public const string UniversalLegalVariableRegEx = @"^[\$\w_][\d\w_.\$]$"; // match a variable string
        public const string UniversalLegalNumericRegEx = @"^(+|-)\d*\.?\d+$";	// match a number

        private string[] delimiters = null;
        private char escapeChar = emptyChar, verbatimChar = emptyChar;
        private Regex _legalVarRegEx = null;	// this is a pattern for most language
        private Regex _legalNumericRegEx = null;

        public void loadGrammar(ILoader loader) {
            ILoader _grammar = loader;
            // if the delimiters are not defined, this detector is not activated
            delimiters = _grammar[PairedTokenDelimitersKey] as string[];
            if (delimiters == null || delimiters.Length < 1) {
                delimiters = null;
                return;
            }
            // load some extra information
            escapeChar = (char)_grammar[EscapeCharKey];
            verbatimChar = (char)_grammar[VerbatimCharKey];
            if (_grammar[LegalVariableRegExKey] != null)
                _legalVarRegEx = new Regex(_grammar[LegalVariableRegExKey] as string, RegexOptions.Compiled);
            else
                _legalVarRegEx = new Regex(UniversalLegalVariableRegEx, RegexOptions.Compiled);
            if (_grammar[LegalNumericRegExKey] != null)
                _legalNumericRegEx = new Regex(_grammar[LegalNumericRegExKey] as string, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            else
                _legalNumericRegEx = new Regex(UniversalLegalNumericRegEx, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public void handle(Tokenizer.Tokenizer t) {
            // exit when there is no delimiters defined
            if (delimiters == null)
                return;

            string _token = t.content; // a string that should be matched before a token is completed

            // eat the verbatim char, replace the unescaped chars with escaped ones in the string (for language like C#)
            if (verbatimChar != emptyChar && _token.StartsWith(verbatimChar.ToString())) {
                _token = _token.Substring(1);	// eat it!
                if (escapeChar != emptyChar)	// double the escaped chars
                    _token = _token.Replace(escapeChar.ToString(), (escapeChar + escapeChar).ToString());
            }

            // when the token is start with some paired delimiters
            for (int i = 0, dLen = delimiters.Length, tLen = _token.Length, strLen = delimiters[i].Length + delimiters[i + 1].Length;
                i < dLen - 1;
                i += 2, strLen = delimiters[i].Length + delimiters[i + 1].Length) {
                // current length of the string is even smaller than the length of paired delimiters's length
                if (tLen < strLen)
                    continue;

                if (_token.StartsWith(delimiters[i])) {
                    // if a match is found
                    if (_token.EndsWith(delimiters[i + 1]) && (escapeChar != emptyChar && !_token.EndsWith(escapeChar + delimiters[i + 1]))) {

                        // normally when a paired delimiter is found, the token is finished, such as "string", 'char', /*comment*/
                        // but in some situation, like RegEx in JavaScript, it is also legal for some expressions to have some appendix, such as /regex/ig, which should be recognized as a token
                        // thus, this Detector should be overrided if you want to use it in JavaScript
                        t.status = Tokenizer.Tokenizer.TokenStatus.COMPLETED;
                        return; // score! exit
                    }
                    else	// the expression is open, wait for its partner
                        return;
                }
            }
            // when a paired delimiter match is not found, see if the token is a legal variable
            // if not, which means it is broken by any punctator, the token will be marked as completed
            if (!_legalVarRegEx.IsMatch(_token) && !_legalNumericRegEx.IsMatch(_token)) {
                t.status = Tokenizer.Tokenizer.TokenStatus.COMPLETED;
            }

        }
    }
}
