

/*
 * This is a generic grammar loader that includes some common style of most coding languages.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar {
    abstract class ALoader : ILoader {

        protected Hashtable _attr = new Hashtable();

        public object this[string index] {
            get {
                return _attr[index];
            }
        }


        // determine if this is a grammar punctuation
        public bool isGrammarPunctuation(char chr) {
            return !chr.Equals('_') && Char.IsPunctuation(chr);
        }
        public bool isKeyword(string str) {
            return false;
        }
    }
}
