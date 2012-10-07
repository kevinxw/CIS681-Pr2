using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Blockader;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.Rules.BlockRules {
    class BlockTypeDetector : IBlockRule {
        public const string BlockTypeKey = "block.type";

        public const string FileType = "file";
        public const string SectionType = "section";


        public void loadGrammar(ILoader loader) {
        }
        public void handle(Block block) {
        }

        // set block type manually
        public static void setType(Block b, string t) {
            b[BlockTypeKey] = t;
        }
    }
}
