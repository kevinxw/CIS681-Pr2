using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Kevin.CIS681.Project.CodeAnalyzer.Exceptions {
    [Serializable()]
    class BlockNotFoundException :Exception {
                private const string msg = "Specific block not found.";

    public BlockNotFoundException() : base() { }
    public BlockNotFoundException(string message=msg) : base(message) { }
    public BlockNotFoundException(string message, System.Exception inner) : base(message, inner) { }

    protected BlockNotFoundException(SerializationInfo info,
        StreamingContext context) { }
    }
}
