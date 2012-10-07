using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Blockader;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.Rules.BlockRules {
    class BlockIntegrityDetector :IBlockaderRule {
		public const string PairedBlockDelimitersKey = "block.pairedBlockDelimiters";
	
	
        private ILoader _grammer;
		
        private string[] delimiters = null;
        
        public void loadGrammar(ILoader loader) {
            _grammer = loader;
            delimiters = _grammer[PairedBlockDelimitersKey] as string[];
        }

        public void handle(Blockader.Blockader b) {
			// determine whether a block is closed is easy, when the first token of this block is a block delimiter, such as "(" and "{"
			// its children will be link to "inner" chain.  And the block will be marked as completed immediately after the delimiter pair is found
            if (true) { // link in
                b.pointer = Blockader.Blockader.Pointer.INNER;
            }
            else if (false) {   // link out
                b.pointer = Blockader.Blockader.Pointer.OUTTER;
            }
        }
    }
}
