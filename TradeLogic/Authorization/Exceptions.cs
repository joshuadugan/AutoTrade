using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLogic.Exceptions
{

    [Serializable]
    public class DeserializeException : Exception
    {
        public string? Xml { get; set; }

        public DeserializeException() { }
        public DeserializeException(string message) : base(message) { }
        public DeserializeException(string message, Exception inner) : base(message, inner) { }
        public DeserializeException(string message, string xml, Exception inner) : base(message, inner)
        {
            Xml = xml;
        }
        protected DeserializeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
