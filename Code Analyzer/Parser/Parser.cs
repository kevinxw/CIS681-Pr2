/*
 * Read information from a code file and try to turn it into a C# data structure
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Blockader;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.Rules.BlockRules;
using Kevin.CIS681.Project.CodeAnalyzer.IO;
using System.Threading;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser {
    class Parser {
		private TextReader tr = null;	// the text reader used to parse current code file
        private string filePath = null;
        private bool _isTextFile = true;    // whether the source file is a text file
        private FileStream fs;

        private List<Block> blockSet = new List<Block>();   // the set of all blocks, the 1st block is the file block

        private Blockader.Blockader fileReader=null;   // the 1st blockader, namely the file block
	
		public Parser (string filePath) {
            Console.Out.WriteLine("Thread ID {0} is parsing file {1}. ", Thread.CurrentThread.ManagedThreadId,filePath);
			if (filePath==null)
				return;
			// read file
            fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            tr = new StreamReader(this.filePath = filePath);
            if (!isTextFile())
                throw new FileLoadException("Target file ["+filePath+"] is not a text file, cannot be analyzed.");
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

        private bool isTextFile() {
            byte[] byteData = new byte[1];
            while (_isTextFile && fs.Read(byteData, 0, byteData.Length) > 0)
                if (byteData[0] == 0)
                    _isTextFile = false;
            return _isTextFile;
        }
    }
}
