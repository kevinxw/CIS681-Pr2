/*
 * mark the block when the block is finished
 */
 
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Blockader;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.Rules.BlockRules {
    interface IBlockaderRule : IRule{
        void handle(Blockader.Blockader b);
    }
}
