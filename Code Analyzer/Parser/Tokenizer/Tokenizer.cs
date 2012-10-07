
/*
 * Tokenizer is a module that extract tokens from Text Reader
 */

//#define TEST_TOKENIZER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Tokenizer {
    class Tokenizer {
        public enum TokenStatus { NULL, COMPLETED }   // there are two status, NULL means the token is not initialized, COMPLETED means it is finished.

        private StringBuilder sb = new StringBuilder();  // current token read buffer.
        private TextReader tr = null;   // text reader
        private Token t=new Token();
        private TokenStatus _tokenStatus = TokenStatus.NULL;

        public delegate void tokenizerEventHandler(Tokenizer t);   // triggered when a new char is read, decide what tokenizer should do next
        public delegate void tokenEventHandler(Token t);  // triggered when the token is finished, add some additional information to the token

        public event tokenEventHandler tokenHandler;
        public event tokenizerEventHandler tokenizerHandler;

        private int _whitespaceCount = 0;   // the number of whitespace locating at the beginning of the string, can be used to detect special structure, such as EOF
        
        public Tokenizer(string t) {
            push(t);
        }
        // read a token from a TextReader (StringReader / StreamReader)
        public Tokenizer(TextReader tr) {
            this.tr = tr;
        }

        public Tokenizer(TextReader tr, char firstChar)
            : this(tr) {
            push(firstChar);
        }
        public int whitespaceCount {
            get { return _whitespaceCount; }
        }
        // get the content currently buffered by the Tokenizer
        public string content {
            get {
                return sb.ToString();
            }
        }

        public StringBuilder buffer {
            get {
                return sb;
            }
        }

        public Token token {
            get {
                return t;
            }
        }

        // push one new char to current token
        // return a boolean value telling if current token is completed
        public TokenStatus push(char chr) {
            // ignore the whitespace in the beginning of the token
            if (sb.Length < 1 && Char.IsWhiteSpace(chr)) {
                _whitespaceCount++;
                return _tokenStatus;
            }
            sb.Append(chr); // read char into buffer
            // actually, the handle can be invoked asynchronizely, but will it take more time initializing new thread than the time we saved?
            tokenizerHandler(this);
            return _tokenStatus;
        }

        // not recommended.  as the token could be completed before all the char array is iterated
        public TokenStatus push(char[] chrs) {
            foreach (char c in chrs) {
                if (_tokenStatus == TokenStatus.COMPLETED)
                    return _tokenStatus;
                push(c);
            }
            return _tokenStatus;
        }
        public TokenStatus push(string str) {
            return push(str.ToCharArray());
        }
        public TokenStatus push(int chr) {
            if (chr == -1)
                return (_tokenStatus = TokenStatus.COMPLETED);
            else
                return push((char)chr);
        }

        // read from TextReader, while not stop until a Token is 
        // return one char which causes the break of a token
        public int read() {
            int chr = -1;
            while (_tokenStatus != TokenStatus.COMPLETED && (chr = tr.Read()) != -1)
                push(chr);
            return chr;
        }
        // read one character from buffer 1st, then continue reading
        public int read(int chr) {
            if (push(chr) != TokenStatus.COMPLETED)
                return read();
            else return tr.Read();
        }

        // whether this token has been completed (whether its block is completed)
        public TokenStatus status {
            get { return _tokenStatus; }
            // mark current token as completed
            set {
                if (_tokenStatus != value)
                    if ((_tokenStatus = value) == TokenStatus.COMPLETED) {
                    t.content = sb.ToString();
                    tokenHandler(t);
                }
            }
        }

#if (TEST_TOKENIZER)
        public static void Main() {
            string testCodeFile = @"x:\test.cs";
            StreamReader sr = new StreamReader(testCodeFile);
            Tokenizer ter = new Tokenizer(sr);
        }
#endif
    }

}
