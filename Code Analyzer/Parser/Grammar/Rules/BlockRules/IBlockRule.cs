
﻿/*
 * mark the block when a token is read in.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.Parser.Blockader;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser.Grammar.Rules.BlockRules {
    interface IBlockRule :IRule{
        void handle(Block b);
    }
}
