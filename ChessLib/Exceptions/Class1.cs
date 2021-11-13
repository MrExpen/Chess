using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Exceptions
{

    [Serializable]
    public class CannotMoveException : Exception
    {
        public CannotMoveException() { }
        public CannotMoveException(string message) : base(message) { }
        public CannotMoveException(string message, Exception inner) : base(message, inner) { }
        protected CannotMoveException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
