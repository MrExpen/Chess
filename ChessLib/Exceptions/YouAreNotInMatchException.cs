using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Exceptions
{

    [Serializable]
    public class YouAreNotInMatchException : Exception
    {
        public YouAreNotInMatchException() { }
        public YouAreNotInMatchException(string message) : base(message) { }
        public YouAreNotInMatchException(string message, Exception inner) : base(message, inner) { }
        protected YouAreNotInMatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
