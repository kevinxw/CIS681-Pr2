using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.Rules.TokenRules;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.Rules.BlockRules;
using Kevin.CIS681.Project.CodeAnalyzer.IO;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.CSharp {
    class CSharpLoader : ALoader {
        // the keywords in C#. see MSDN doc here: http://goo.gl/aRqWT
        // I delete the "in (generic modifier)" & "out (generic modifier)" here
        // this list actually can be gained by concatenating the const _types, _operators, _statements etc., but as MSDN gives a full list, I won't bother doing this
        private const string _keywords = "abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while";

        // types http://goo.gl/cXlUW
        private const string _types = "bool|byte|sbyte|char|decimal|double|float|int|uint|long|ulong|object|short|ushort|string";

        // statements http://goo.gl/LRY7Q
        private const string _statements =
            "if|else|switch|case" +   // selection, array[0-3]
            "|do|for|foreach|in|while" + // iteration, array[4-8]
            "|break|continue|default|goto|return|yield" +    // jump, array[9-14]
            "|throw|try-catch|try-finally|try-catch-finally" +   // exception handling, array[15-18]
            "|checked|unchecked" +   // checked and unchecked, array[19-20]
            "|fixed" +   // fixed, array[21]
            "|lock"    // locked, array[22]
            ;

        // preprocessor derectives, see http://goo.gl/E48ZG
        private const string _preprossors =
            "#if|#else|#elif|#endif" +   // selection, array[0-3]
            "|#define|#undef" +  // declration, array[4-5]
            "|#warning|#error" + // exception handling, array[6-7]
            "|#line" +
            "|#region|#endregion" +
            "|#pragma|#pragma warning|#pragma checksum"	// pay attention to the waring & checksum keywords, they are two-word
            ;

        // every other element in this array make one pair
        private const string _pairedTokenDelimiters =
            "\"|\"" +    // a quoted string
            "|\'|\'" +   // a char
            "|/*|*/" +    // a comment
            "|//|\n"   // a single-line comment
            ;
        private const string _pairedBlockDelimiters =
            "{|}"	// barce, used in function, set, etc.
            ;
        private const string _pairedParameterDelimiters =
            "(|)"	// used in function call (with or without parameters), or for (), while ()
            ;

        private const char _escapeChar = '\\';    // escape character that used in a quoted string, disable double quote with \", and disable single quote with \'.  pay attention to \\
        private const char _verbatimChar = '@';   // disable the escape character anyway.. used in the start of a quoted string, like @"some words", or in front of some types, like @int, @class
        private const string _legalVariableRegEx = @"^@?[\w_][\d\w._]*$";	// the regular expression of a legal variable.  It does vary from language, e.g. "@" seems is legal only in C#, and "$" is legal in a lot of language, while it doesn't work in C#
        private const string _legalNumbericRegEx = @"^(+|-)\d*\.?\d+(E\+\d+|f|d|m|l|u|ul)$";	// the regular expression of a legal number, including double / float / int etc.
        private const string _fileExt = ".cs";

        // open to the public
        public CSharpLoader() {
            _attr.Add(FileManager.CodeFileExtKey, _fileExt.Split('|'));
            _attr.Add(TokenTypeDetector.TokenKeywordKey, _keywords.Split('|'));
            _attr.Add(TokenTypeDetector.TokenTypeKey, _types.Split('|'));
            _attr.Add(TokenTypeDetector.TokenStatementKey, _statements.Split('|'));
            _attr.Add(TokenTypeDetector.TokenPreProssorKey, _preprossors.Split('|'));
            // here I make the parameter return as one token, not recommanded when do "real analyzing", but does simplify the processing
            _attr.Add(TokenIntegrityDetector.PairedTokenDelimitersKey, _pairedTokenDelimiters.Split('|').Concat(_pairedParameterDelimiters.Split('|')));
            _attr.Add(TokenIntegrityDetector.EscapeCharKey, _escapeChar);
            _attr.Add(TokenIntegrityDetector.VerbatimCharKey, _verbatimChar);
            _attr.Add(TokenIntegrityDetector.LegalVariableRegExKey, _legalVariableRegEx);
            _attr.Add(TokenIntegrityDetector.LegalNumericRegExKey, _legalNumbericRegEx);
            _attr.Add(BlockIntegrityDetector.PairedBlockDelimitersKey, _pairedBlockDelimiters.Split('|'));
            _attr.Add(TokenTypeDetector.TokenParameterKey, _pairedParameterDelimiters.Split('|'));
        }
    }
}
