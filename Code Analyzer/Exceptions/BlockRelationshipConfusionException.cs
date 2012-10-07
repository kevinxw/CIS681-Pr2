/*
 * This exception will be thrown out when there is an illegal operation happens to a block
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Kevin.CIS681.Project.CodeAnalyzer.Exceptions {
    [Serializable()]
    class BlockRelationshipConfusionException : Exception {
        private const string msg = "There is a fatal error in your current operation.  A block relationship confusion happens and the Analyzer will fail to understand.";

    public BlockRelationshipConfusionException() : base() { }
    public BlockRelationshipConfusionException(string message=msg) : base(message) { }
    public BlockRelationshipConfusionException(string message, System.Exception inner) : base(message, inner) { }

    protected BlockRelationshipConfusionException(SerializationInfo info,
        StreamingContext context) { }
    }
}
