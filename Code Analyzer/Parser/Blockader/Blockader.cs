using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Tokenizer;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Blockader {
    class Blockader {
		private bool _isCompleted = false;
        private TextReader tr = null;

		private Block _current = new Block(),  // current block
            _parent = null, // parent block, refers to the current block's upper level
            _previous = null;   // previous processed block, including its upper level block and previous block

        public enum Pointer { NEXT, PREVIOUS, INNER, OUTTER, NULL };

        // the pointer tell blockader which direction to go next
        // when it is null, mean this block has no sibling or children, should return to its parents
        private Pointer _pointer = Pointer.NULL;
		
		public delegate void blockEventHandler(Block b);
		public delegate void blockaderEventHandle(Blockader b);
		
        public event blockEventHandler blockHandler;
        public event blockaderEventHandle blockaderHandler;

        public Pointer pointer {
            get {
                return _pointer;
            }
            set {
                _pointer = value;
            }
        }

        // read next block, return false when there is no more block
		public bool next(int chr) {
            if (chr == -1) return false;
            Blockader blocker = new Blockader(tr);
            switch (_pointer) {
                case Pointer.INNER:
                    // jump in

                    return blocker.next((this + blocker).read(chr));
                case Pointer.OUTTER:
                    // return to upper level
                    return blocker.next((this - blocker).read(chr));
                case Pointer.PREVIOUS:
                    // jump to the previous block, normally this operation will not be used
                    return blocker.next((this < blocker).read(chr));
                case Pointer.NEXT:
                default:
                    return blocker.next((this > blocker).read(chr));
            }
		}

        // link in
        public static Blockader operator +(Blockader b1, Blockader b2) {
            b1.currentBlock.linkIn(b2.currentBlock);
            return b2;
        }
        public static Blockader operator -(Blockader b1, Blockader b2) {
            b1.currentBlock.linkOut(b2.currentBlock);
            return b2;
        }
        public static Blockader operator >(Blockader b1, Blockader b2) {
            b1.currentBlock.next(b2.currentBlock);
            return b2;
        }
        public static Blockader operator <(Blockader b1, Blockader b2) {
            b1.currentBlock.previous(b2.currentBlock);
            return b2;
        }
		
		public bool isCompleted {
			get{
			    return _isCompleted;
			}
			set {
                if (_isCompleted = value) {
                    blockHandler(_current);

                }
			}
		}

        public Block parentBlock {
            get {
                return _parent;
            }
            set {
                _parent = value;
            }
        }
        public Block currentBlock {
            get {
                return _current;
            }
            
        }
        public Block previousBlock {
            get {
                return _previous;
            }
            set {
                _previous = value;
            }
        }

        // initialize TextReader
		public Blockader(TextReader tr) {
			this.tr = tr;
		}
		
		public Blockader (TextReader tr, Token firstToken) : this(tr) {
            push(firstToken);
		}

        // read from TextReader, which is, initialize a new Tokenizer
        // return until this block is completed
        public int read() {
            int chr=-1;
            Tokenizer.Tokenizer toker = new Tokenizer.Tokenizer(tr);
            while (!_isCompleted && (chr = toker.read()) != -1)
                push(toker.token);
            if (chr == -1)
                isCompleted = true;
            return chr;
        }
        public int read(int chr) {
            if (chr == -1) {
                isCompleted = true;
                return chr;
            }

            else {
                Tokenizer.Tokenizer toker = new Tokenizer.Tokenizer(tr,(char)chr);
                while (!_isCompleted && (chr = toker.read()) != -1)
                    push(toker.token);
                if (chr == -1)
                    isCompleted = true;
                return chr;
            }
        }

        // push token to current block
        // return a boolean value telling whether the block is completed
        public bool push(Token t) {
            _current.push(t);
            blockaderHandler(this);
            return _isCompleted;
        }
		
		// pass current token to its parent
		public void pop() {
		}
    }
}
