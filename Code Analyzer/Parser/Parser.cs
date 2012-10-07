/*
 * Read information from a code file and try to turn it into a C# data structure
 * 
 */

//#define TEST_PARSER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Blockader;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.Rules.BlockRules;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser {
    class Parser {
		private TextReader tr = null;	// the text reader used to parse current code file
		private string filePath = null;

        private List<Block> blockSet = new List<Block>();   // the set of all blocks, the 1st block is the file block

        private Blockader.Blockader fileReader=null;   // the 1st blockader, namely the file block
	
		public Parser (string filePath) {
			if (filePath==null)
				return;
			// read file
			tr = new StreamReader(this.filePath = filePath);
			// initialize the file block

		}
		
		// load parser from a existing TextReader
		public Parser (TextReader tr) {
			this.tr = tr;
		}

        // start reading file
        public void read() {
            fileReader = new Blockader.Blockader(tr);
            fileReader.pointer = Blockader.Blockader.Pointer.INNER;
            // read the file content
            fileReader.next(tr.Read());
            BlockTypeDetector.setType(fileReader.currentBlock, BlockTypeDetector.FileType);
        }

        // return the file block
        public Block fileBlock {
            get {
                return fileReader.currentBlock;
            }
        }

#if (TEST_PARSER)
        public static void Main() {
            string testCodeFile = @"x:\test.cs";
            StreamReader sr = new StreamReader(testCodeFile);
            Parser p = new Parser(sr);
        }
#endif
    }
}
