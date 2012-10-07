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
    class Block: IXmlSerializable {
		private Hashtable _attr = new Hashtable();
        private Block _inner = null,
           _outter = null,
           _next = null,
           _previous = null;
        private List<Token> _tokens = new List<Token>(); // the tokens that included in this block
        private List<Block> _children = new List<Block>();  // children blocks
        private string _id=null;  // the block id should be unique in one project.  It can be used to determine if two blocks are the same

        public string Id {
            get {
                return _id;
            }
        }
        public Block() {
            _id = generateBID();
        }

        public Block(string bid) {
            _id = bid;
        }

	    // get attribute
        public object this[string index] {
            get {
                return _attr[index];
            }
            set {
                _attr[index] = value;
            }
        }
        // return one's inner link
        public Block linkIn() {
            return _inner;
        }
        public Block linkOut(Block b) {
            return _outter = b;
        }
        public Block linkOut() {
            return _outter;
        }
        // b1 & b2 shall not be null
        public static Block operator >(Block b1, Block b2) {
           return !b1 ? null : (!b2 ? b1.next(null) : b2.previous(b1).next(b2));
        }
        public static Block operator <(Block b1, Block b2) {
            return !b1 ? null : (!b2 ? b1.previous(null) : b2.next(b1).previous(b2));
        }
		
		// push one token to current block, and return itself
		public Block push(Token t) {
			_tokens.Add(t);
			return this;
		}
		
		public Token shift() {
			return _tokens[_tokens.Count-1];
		}
		
		// push/append a token to one block
		public static Block operator <(Block b, Token t) {
			return b.push(t);
		}
		
		public static Block operator >(Block b, Token t) {
			b.shift().copyTo(t);
            return b;
		}

        public List<Token> tokens {
            get {
                return _tokens;
            }
        }
        public Block linkIn(Block b) {
            return _inner = b;
        }

        // the operator for linkIn / linkOut, pay attention linkIn & linkOut are one-way direction
        public static Block operator +(Block b1, Block b2) {
            return !b1 ? null : b1.linkIn(b2);
        }
        public static Block operator -(Block b1, Block b2) {
            return !b1 ? null : b1.linkOut(b2);
        }

        public static Block operator ++(Block b) {
            return b.next();
        }
        public static Block operator --(Block b){
            return b.previous();
        }
        public static Block operator +(Block b) {
            return b.linkIn() | b.next();
        }
        public static Block operator -(Block b) {
            return b.previous() | b.linkOut();
        }
        public static Block operator |(Block b1, Block b2) {
            return !b1 ? b2 : b1;
        }
        public static Block operator &(Block b1, Block b2) {
            return (!b1 && !b2) ? b2 : null;
        }

        // determine if one block is null
        public static bool operator !(Block b) {
            return (b == null || b._id==null);
        }
        // get one's MOST precedent block
        public static Block operator <<(Block b, int i) {
            for (; i > 0 && !!b; i--)
                b = b.previous();
            return b;
        }
        public static Block operator >>(Block b, int i) {
            for (; i > 0 && !!b; i--)
                b = b.next();
            return b;
        }

        public Block next(Block b) {
            return _next = b;
        }
        public Block next() {
            return _next;
        }
        public Block previous(Block b) {
            return _previous = b;
        }
        public Block previous() {
            return _previous;
        }
        // attention the difference!
        // operator <<>> will throw no exception, while <> will
        public static Block operator <(Block b, int i) {
            for (; i > 0; i--) {
                if (!b)
                    throw new BlockNotFoundException();
                b = b.previous();
            }
            return b;
        }
        public static Block operator >(Block b, int i) {
            for (; i > 0; i--) {
                if (!b)
                    throw new BlockNotFoundException();
                b = b.next();
            }
            return b;
        }
        // if the two blocks are the same
        // ATTENTION! this method only compare the Ids, instead of every member values of the two blocks
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
        private string generateBID() {
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
            return "Block#"+_id;
        }




#if DEBUG_BLOCK
        [STAThread]
        static void Main(string[] args) {
        }
#endif
    }
}
