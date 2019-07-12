using System;
using System.Runtime.Serialization;

namespace _7Sharp
{
    [Serializable]
    internal class DeprecatedException : Exception
    {
        public DeprecatedException()
        {
        }

        public DeprecatedException(string message) : base(message)
        {
        }

        public DeprecatedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeprecatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}