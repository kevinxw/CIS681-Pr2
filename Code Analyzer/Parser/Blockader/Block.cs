/*
 * Block is a data struct that stores the structure of a code file.
 * A code file can be translated into a set of blocks, which is a multiply linked list, which can also be regarded as a graph
 * (see the structure schematic diagram here http://goo.gl/z6C0S)
 * 
 * You may take it as the semi-expression in Prof. Fawcett's sample project, regardless of some structure difference.  But it actually does the same job.
 * 
 * Block multiply linked list can fully describe the information of a code file, by inspecting it, we can have a overall conception of a code file except calculating the cyclomatic complexity
 * 
 */

// debug the block
//#define DEBUG_BLOCK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Kevin.CIS681.Project.CodeAnalyzer.Exceptions;
using Kevin.CIS681.Project.CodeAnalyzer.Utils;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Tokenizer;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Blockader {
    [Serializable]
    class Block : IXmlSerializable {
        private static List<Block> blockSet = new List<Block>();


        private Dictionary<string, string> _attr = new Dictionary<string, string>();    // attribute dictionary, key and value must be string (for Serialization)
        private List<Token> _tokens = new List<Token>(); // the tokens that included in this block
        private List<Block> _children = new List<Block>();  // children blocks
        private Block _parent = null;   // the parent of this block, null means this is a file block
        private string __id = null;  // the block id should be unique in one project.  It can be used to determine if two blocks are the same

        public string Id {
            get {
                return _id;
            }
        }

        private string _id {
            get {
                return __id;
            }
            set {
                this["id"] = __id = value;
            }
        }
        public Block() {
            _id = generateBID();
        }

        public Block(string bid) {
            _id = bid;
        }

        // get attribute
        public string this[string index] {
            get {
                return _attr.ContainsKey(index) ? _attr[index] : String.Empty;
            }
            set {
                _attr[index] = value;
            }
        }

        public string[] attributes {
            get {
                return _attr.Keys.ToArray();
            }
        }

        // return the children of one block
        public List<Block> children {
            get {
                // this is not a copy, means its content can be changed publicly!
                return _children;
            }
        }

        // push one token to current block, and return itself
        public Block push(Token t) {
            _tokens.Add(t);
            return this;
        }

        public Token shift() {
            return _tokens[_tokens.Count - 1];
        }

        // push/append a token to one block
        public static Block operator <(Block b, Token t) {
            return b.push(t);
        }

        public static Block operator >(Block b, Token t) {
            b.shift().copyTo(t);
            return b;
        }

        public static Block operator <(Block p, Block c) {
            p.children.Add(c);
            return p;
        }
        public static Block operator >(Block c, Block p) {
            p.children.Add(c);
            return c;
        }

        public List<Token> tokens {
            get {
                return _tokens;
            }
        }

        public bool hasChild {
            get {
                return _children.Count > 0;
            }
        }

        // determine if one block is null
        public static bool operator !(Block b) {
            return (b == null || b._id == null);
        }

        // ATTENTION! this method only compare the Ids, instead of every member value
        public static bool operator ==(Block b1, Block b2) {
            return !!b1 && !!b2 && b1._id == b2._id;
        }
        public static bool operator !=(Block b1, Block b2) {
            return !b1 || !b2 || b1._id != b2._id;
        }

        override public bool Equals(Object o) {
            Block b = o as Block;
            if (!b)
                return false;
            else
                return (this == b);
        }

        // make a clone of current Block
        public Block clone() {
            //Block b = new Block(_id);
            return null;
        }


        // generate block id
        /*
         * block id is a randomly generated character sequence
         */
        public static string generateBID() {
            return Text.getRandomString(32);
        }


        // Xml Serialization Infrastructure ==========================================

        public void WriteXml(XmlWriter w) {
            if (!this)
                return;
            w.WriteStartElement(GetType().ToString());
            w.WriteAttributeString("id", _id);
            w.WriteEndElement();
        }

        public void ReadXml(XmlReader r) {
            //personName = r.ReadString();
        }

        public XmlSchema GetSchema() {
            return (null);
        }

        // Print when debug
        public override string ToString() {
            return "Block#" + _id;
        }

#if DEBUG_BLOCK
        [STAThread]
        static void Main(string[] args) {
        }
#endif
    }
}
