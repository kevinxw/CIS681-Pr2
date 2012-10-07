
/*
 * class "Token" is just a data structure, but also a reader that extract tokens from a TextReader object
 * see http://goo.gl/dhrcQ
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Blockader;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Tokenizer {
    [Serializable]
    class Token : IXmlSerializable {
        public enum Type { LineTerminators, Comment, Keyword };
        private string _token = null;   // the content of one token, notice to save memory and performance, this is not a StringBuffer
        private Hashtable _attr = new Hashtable();    // record the attributes of one token

        public Token(string t) : this() {
            content = t;
        }
        public Token() {
        }

        public string content {
            get { return _token; }
            set { _token = value.Trim(); }
        }

        public object this[string index] {
            get {
                return _attr[index];
            }
            set {
                _attr[index] = value;
            }
        }
		
		// shallow copy data to target from current token
		public Token copyTo(Token t) {
			return t;
		}

        public ICollection attributes {
            get {
                return _attr.Keys;
            }
        }

        // get token
        public string toString() {
            return _token;
        }

        // Xml Serialization Infrastructure 

        public void WriteXml(XmlWriter writer) {
        }

        public void ReadXml(XmlReader reader) {
        }

        public XmlSchema GetSchema() {
            return (null);
        }
    }
}
